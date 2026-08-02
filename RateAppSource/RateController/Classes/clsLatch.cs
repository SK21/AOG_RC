using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Classes
{
    // Shared latching for the small always-on-top windows - the product displays, the
    // pressure display and the switch panel. A latched window is owned by the window it
    // was dropped on and moves with it, so the operator can build a cluster and drag the
    // whole thing around as one.
    //
    // A window can latch to the main form (or to the RCRestore mini window while the main
    // form is minimized), and equally to any other latching window. Chains need no special
    // handling: moving a window raises its own LocationChanged, which drags whatever is
    // latched to it, and so on down the chain.
    //
    // One instance per latching form. The form supplies the events - see the members below
    // for what has to be called from where.
    public class clsLatch
    {
        private static readonly List<clsLatch> Latches = new List<clsLatch>();

        private readonly Form Frm;
        private bool IsManuallyMoved = false;
        private Point Offset;
        private Form Target = null;
        private bool TargetAttached = false;

        public clsLatch(Form LatchingForm)
        {
            Frm = LatchingForm;
            Latches.Add(this);
        }

        // Latched means this form is currently owned by, and following, its target. A form
        // can have a target it is not latched to - it stays a candidate until they overlap.
        public bool IsLatched
        { get { return Frm.Owner != null && Frm.Owner == Target; } }

        // Called on the form's MouseUp: the user has dropped it, so see what it landed on.
        public void Dropped()
        {
            IsManuallyMoved = false;
            TryToLatch();
        }

        // Called when the main form minimizes. Only the link to the main form is dropped,
        // so the window stays visible; a cluster latched window-to-window stays together
        // because those owners are not minimizing.
        public void HostMinimized()
        {
            try
            {
                if (Frm.Owner != null && Frm.Owner == Core.MainForm) Frm.Owner = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsLatch/HostMinimized: " + ex.Message);
            }
        }

        // Called from the form's LocationChanged. A drag by hand breaks the latch until the
        // drop re-evaluates it; a move made by the target dragging us along must not.
        public void Moved()
        {
            try
            {
                if (IsManuallyMoved && IsLatched) Frm.Owner = null;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsLatch/Moved: " + ex.Message);
            }
        }

        // Called from the drag handler while the mouse is down.
        public void MovedByHand()
        {
            IsManuallyMoved = true;
        }

        // Re-reads the offset after the form changes size, so the visual relationship the
        // user set up is kept rather than the window jumping on the target's next move.
        public void Refresh()
        {
            if (IsLatched) SetOffset();
        }

        // Called on load and whenever the main form is restored: pick a target, latch if
        // the two overlap, and put this window back on top.
        public void Setup()
        {
            try
            {
                TryToLatch();

                if (!Frm.TopMost) Frm.TopMost = true;
                Frm.BringToFront();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsLatch/Setup: " + ex.Message);
            }
        }

        // Called from the form's shutdown. Deliberately leaves Frm.Owner alone: at app exit
        // the main form closing is what destroys the displays latched to it, and a window
        // that let go of its owner here would survive that and keep the process alive. A
        // follower whose target is closing never reaches this - TargetIsClosing catches it
        // first, and that is where letting go is the right move.
        public void ShutDown()
        {
            SetTarget(null);
            Latches.Remove(this);
        }

        // Called from the form's FormClosing, and true when this close is only the latch
        // target going away and taking its owned window along - the form should then stay
        // open, unlatched, instead of shutting down.
        //
        // Windows destroys an owned window with its owner, and WinForms raises the owned
        // form's FormClosing (CloseReason.FormOwnerClosing) BEFORE the owner's own
        // FormClosing runs, so no hook on the target is early enough to head this off. The
        // rescue has to happen here: dropping out of the target's owned-forms list is what
        // lets this window survive the target's destruction.
        //
        // Two things this deliberately does not do. It does not cancel the close, because
        // cancelling an owned form's cascade close cancels the OWNER's close as well, which
        // would leave the window that was just switched off sitting on screen. And it only
        // applies when the owner is another latching window: the main form closing is the
        // application exiting, and the displays have to go with it.
        public bool TargetIsClosing(FormClosingEventArgs e)
        {
            bool Result = false;

            if (e.CloseReason == CloseReason.FormOwnerClosing && FindLatch(Frm.Owner) != null)
            {
                SetTarget(null);

                try
                {
                    Frm.Owner = null;
                    Result = true;
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsLatch/TargetIsClosing: " + ex.Message);
                }
            }

            return Result;
        }

        // Latches to the best available target if the two overlap. Also called by a form
        // that has just resized itself, since that can create or remove the overlap.
        public bool TryToLatch()
        {
            bool Result = false;

            try
            {
                SetTarget(BestTarget());

                // Always drop the owner first; it is reassigned only if they overlap.
                Frm.Owner = null;

                if (Target != null && !Target.IsDisposed && Frm.Bounds.IntersectsWith(Target.Bounds))
                {
                    Frm.Owner = Target;
                    SetOffset();
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsLatch/TryToLatch: " + ex.Message);
            }

            return Result;
        }

        // The host window keeps first claim on anything dropped on it, which is how this
        // behaved before other windows became latch targets. Failing that, the latching
        // window with the biggest overlap wins.
        private Form BestTarget()
        {
            Form Result = null;
            Rectangle Mine = Frm.Bounds;
            Form Host = HostForm();

            if (Host != null && Mine.IntersectsWith(Host.Bounds))
            {
                Result = Host;
            }
            else
            {
                int BestArea = 0;

                foreach (clsLatch Other in Latches)
                {
                    if (Other != this && Other.Frm != null && !Other.Frm.IsDisposed
                        && Other.Frm.Visible && !Follows(Other.Frm))
                    {
                        Rectangle Overlap = Rectangle.Intersect(Mine, Other.Frm.Bounds);
                        int Area = Overlap.Width * Overlap.Height;

                        if (Area > BestArea)
                        {
                            BestArea = Area;
                            Result = Other.Frm;
                        }
                    }
                }

                // Nothing to latch to, but stay pointed at the host so a later drop onto it
                // is picked up by TryToLatch without waiting for a Setup().
                if (Result == null) Result = Host;
            }

            return Result;
        }

        // The latch belonging to a form, or null when the form is not one of ours - which
        // is how the main form and RCRestore are told apart from the latching windows.
        private static clsLatch FindLatch(Form frm)
        {
            clsLatch Result = null;

            if (frm != null)
            {
                foreach (clsLatch Item in Latches)
                {
                    if (Item.Frm == frm)
                    {
                        Result = Item;
                        break;
                    }
                }
            }

            return Result;
        }

        // True when the candidate already follows this form, directly or through a chain.
        // Latching to it would make a loop, which Form.Owner rejects outright. The walk
        // is up the live Owner chain rather than the target chain, because a target is
        // recorded even while the two do not overlap - that is a candidate, not a link.
        // The hop limit is belt and braces in case a loop ever does get built some other
        // way, since this would otherwise spin forever.
        private bool Follows(Form Candidate)
        {
            bool Result = false;
            Form Node = Candidate;
            int Hops = 0;

            while (Node != null && Hops <= Latches.Count + 1)
            {
                if (Node == Frm)
                {
                    Result = true;
                    break;
                }
                Node = Node.Owner;
                Hops++;
            }

            return Result;
        }

        private Form HostForm()
        {
            Form Result = null;

            if (Core.MainForm != null && !Core.MainForm.IsDisposed
                && Core.MainForm.WindowState != FormWindowState.Minimized)
            {
                Result = Core.MainForm;
            }
            else
            {
                Result = Props.IsFormOpen("RCRestore", false);
            }

            return Result;
        }

        private void SetOffset()
        {
            Offset = new Point(Frm.Location.X - Target.Location.X,
                               Frm.Location.Y - Target.Location.Y);
        }

        private void SetTarget(Form NewTarget)
        {
            if (NewTarget != Target)
            {
                try
                {
                    if (TargetAttached && Target != null)
                    {
                        Target.LocationChanged -= TargetMoved;
                        Target.FormClosing -= TargetClosing;
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsLatch/SetTarget detach: " + ex.Message);
                }
                finally
                {
                    TargetAttached = false;
                }

                Target = NewTarget;

                try
                {
                    if (Target != null && !Target.IsDisposed)
                    {
                        Target.LocationChanged += TargetMoved;
                        Target.FormClosing += TargetClosing;
                        TargetAttached = true;
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsLatch/SetTarget attach: " + ex.Message);
                }
            }
        }

        private void TargetClosing(object sender, FormClosingEventArgs e)
        {
            // The target is going away; drop the ownership so this window survives it.
            SetTarget(null);
            Frm.Owner = null;
        }

        private void TargetMoved(object sender, EventArgs e)
        {
            try
            {
                if (Target != null && !Target.IsDisposed && IsLatched)
                {
                    Point Desired = new Point(Target.Location.X + Offset.X,
                                              Target.Location.Y + Offset.Y);

                    if (Frm.Location != Desired)
                    {
                        Point WasAt = Frm.Location;
                        Frm.Location = Desired;

                        // Revert if the new location is off-screen
                        if (!Props.IsOnScreen(Frm, false)) Frm.Location = WasAt;

                        // Bring to front (less flicker than toggling TopMost)
                        if (!Frm.TopMost) Frm.TopMost = true;
                        Frm.BringToFront();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsLatch/TargetMoved: " + ex.Message);
            }
        }
    }
}
