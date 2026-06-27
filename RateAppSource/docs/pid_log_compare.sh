#!/usr/bin/env bash
# pid_log_compare.sh - classify a RateController PGN32402 PID log.
#
# Tells you, for a given log:
#   1) which firmware produced it  (OLD absolute-error vs NEW normalized/conditional-integration)
#   2) the dominant near-target signature  (CRAWL vs OVERSHOOT/RING vs CLEAN)
#   3) the matching tuning lever
#
# Usage:
#   ./pid_log_compare.sh LOG.csv [brakepoint%]      (brakepoint default 35)
#
# CSV columns (PGN 32402):
#   PCTime,ModuleMillis,ModuleID,SensorID,Target,Applied,Error,Integral,Change,PWM,Samples[,InoID]
#   InoID (12th column) is present on logs from the InoID-change firmware onward; older logs
#   omit it and the script falls back to fingerprinting Change/PWM magnitude.
#
# Run under Git Bash on Windows.

set -euo pipefail
LOG="${1:?usage: pid_log_compare.sh LOG.csv [brakepoint%]}"
BP="${2:-35}"

awk -F, -v BP="$BP" '
BEGIN{
  ENTER=0.08;    # |FracError| above this (fraction of ref) starts a correction episode
  SETTLE=0.04;   # |FracError| at/below this = back at target (closes the episode)
  OVS=0.05;      # crossing target by > OVS*target counts as an overshoot
  CRAWLMS=1500;  # settling slower than this with no overshoot = crawl
  RINGN=2;       # >= this many error sign-reversals in an episode = ringing
  GAPMS=5000;    # row gap bigger than this = new control session (reset gain latch)
}
function abs(x){ return x<0?-x:x }
function closeEpisode(endms,   st,over,frac){
  if(!active) return;
  active=0;
  st = endms - sStart;
  nEp++; sumSettle += st;
  over = (maxOpp > OVS*sTgt) ? 1 : 0;
  frac = (sTgt>0 ? maxOpp/sTgt : 0);
  if(frac > maxOverPct) maxOverPct = frac;
  if(over || reversals>=RINGN){ nOver++;  cls="OVERSHOOT/RING" }
  else if(st>CRAWLMS){          nCrawl++; cls="CRAWL" }
  else {                        nClean++; cls="CLEAN" }
  epLine[nEp]=sprintf("  @%8dms  start_err=%+6.2f  tgt=%5.2f  settle=%5dms  revs=%d  maxOvershoot=%4.1f%%  avgPWM=%3.0f  igMax=%4.1f  -> %s",
     sStart, sErr0, sTgt, st, reversals, 100*frac, (npwm>0?sumPWM/npwm:0), igmax, cls);
}
NR==1{ next }   # header
{
  ms=$2+0; t=$5+0; a=$6+0; e=$7+0; ig=$8+0; ch=$9+0; pwm=$10+0; smp=$11+0;
  if(NF>=12 && $12!=""){ inoid=$12+0; haveIno=1 }   # exact firmware build id when present
  rows++;
  if(abs(ch)>maxch)  maxch=abs(ch);
  if(abs(pwm)>maxpwm)maxpwm=abs(pwm);

  if(haveprev && (ms-prevms)>GAPMS){ closeEpisode(prevms); ref=0 }  # session boundary
  if(t>ref)  ref=t;          # firmware latches peak target as the gain reference
  if(t>gpeak)gpeak=t;

  fr  = (ref>0 ? e/ref : 0);
  afr = abs(fr);

  if(haveprev && afr>BP/100.0 && abs(ig)>abs(previg)+1e-6) windupOut++;  # integral growing far from target
  if(afr>BP/100.0) outBandRows++; else inBandRows++;

  if(!active){
    if(afr>ENTER && t>0){
      active=1; sStart=ms; sSign=(e>=0?1:-1); sTgt=t; sErr0=e;
      maxOpp=0; reversals=0; lastSign=sSign; sumPWM=0; npwm=0; igmax=0;
    }
  }
  if(active){
    sumPWM+=abs(pwm); npwm++;
    if(abs(ig)>igmax)igmax=abs(ig);
    cs=(e>=0?1:-1);
    if(cs!=lastSign){ reversals++; lastSign=cs }
    if(cs!=sSign){ if(abs(e)>maxOpp)maxOpp=abs(e) }   # excursion past target
    if(afr<=SETTLE && t>0) closeEpisode(ms);
  }
  prevms=ms; previg=ig; haveprev=1;
}
END{
  if(active) closeEpisode(prevms);
  inb = inBandRows+outBandRows; if(inb<1) inb=1;
  print  "================ PID LOG COMPARISON ================";
  printf "rows                : %d\n", rows;
  printf "peak target (ref)   : %.2f UPM\n", gpeak;
  printf "brakepoint band     : +/- %.0f%% of ref  (= +/- %.2f UPM)\n", BP, BP/100.0*gpeak;
  printf "time inside band    : %.1f%%      outside (approach mode): %.1f%%\n",
         100*inBandRows/inb, 100*outBandRows/inb;
  print  "---------------------------------------------------------";
  if(maxpwm>130 || maxch>130 || windupOut>5)
    fw="OLD absolute-error firmware (NOT the normalized/conditional-integration build)";
  else
    fw="NEW normalized firmware (consistent with conditional integration)";
  printf "max|Change|=%.0f   max|PWM|=%.0f   integral-grows-outside-band=%d rows\n", maxch, maxpwm, windupOut;
  if(haveIno){
    # InoID encodes ddmmy: year = id%10 + 2020, ddmm = id/10
    yr = (inoid%10) + 2020; rest = int(inoid/10); dd = int(rest/100); mm = rest%100;
    printf "FIRMWARE (InoID)    : %d  (build %02d/%02d/%d) - EXACT; compare to your normalized build id\n", inoid, dd, mm, yr;
    printf "fingerprint cross-check: %s\n", fw;
  } else {
    printf "FIRMWARE            : %s  (inferred - no InoID column in this log)\n", fw;
  }
  print  "---------------------------------------------------------";
  printf "correction episodes : %d   (CRAWL=%d  OVERSHOOT/RING=%d  CLEAN=%d)\n", nEp,nCrawl,nOver,nClean;
  if(nEp>0){
    printf "avg settle time     : %.0f ms\n", sumSettle/nEp;
    printf "worst overshoot     : %.1f%% of target\n", 100*maxOverPct;
    print  "---------------------------------------------------------";
    for(i=1;i<=nEp;i++) print epLine[i];
  }
  print  "==================== VERDICT ============================";
  if(nEp==0)
    print "Pure steady state - no excursions. Need a log with section on/off or rate steps.";
  else if(nOver>nClean && nOver>=nCrawl){
    print "Signature: OVERSHOOT / RINGING dominant.";
    print "Lever  -> NARROW BrakePoint (brake sooner) and/or reduce Kp.";
    print "          Check integral-grows-outside-band above - if >0 on NEW fw, also cut Ki/MaxIntegral.";
  } else if(nCrawl>nClean && nCrawl>=nOver){
    print "Signature: CRAWL into target dominant.";
    print "Lever  -> RAISE PIDslowAdjust (less throttle in band) and/or raise Ki.";
    print "          Widen BrakePoint only as a last resort (reintroduces windup room).";
  } else if(nOver+nCrawl==0)
    print "Signature: CLEAN - approaches and settles without overshoot or crawl. Leave tuning as-is.";
  else
    printf "Signature: MOSTLY CLEAN - %d clean vs %d overshoot / %d crawl. A few outliers; not a dominant pattern.\n", nClean,nOver,nCrawl;
  print  "========================================================";
}
' "$LOG"
