
// VTPool.ino - ISO 11783-6 VT Object Pool Loader
// Pool data is stored in flash (PROGMEM) in VTPoolData.h, generated from an
// .iop file created in AgIsoTerminalDesigner (converted via iop_to_header.ps1).
// On Teensy 4.1 (ARM), PROGMEM is memory-mapped and can be read via normal
// pointers — no SRAM copy needed.

#include "VTPoolData.h"

const uint8_t* VTPool_GetBuffer() {
    return iopPoolData;
}

uint16_t VTPool_GetSize() {
    return IOP_POOL_SIZE;
}
