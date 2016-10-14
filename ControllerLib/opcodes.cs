using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopazControllerLib
{
    enum OPCODES : ushort
    {

        OPC_SET_FREQUENCY,
        OPC_SET_MAGNITUDE,
        OPC_READ_DETECTOR,
        OPC_GET_VERSION,

        OPC_ADDALGO,
        OPC_APPENDALGO,
        OPC_CLEARALGOS,
        OPC_START_COOKING,
        OPC_STOP_COOKING,
        OPC_PAUSE_COOKING,
        OPC_START_DISH_COOKING,


        OPC_WRITE_TOPAZ_INFO,
        OPC_WRITE_CAL_AB_LINEAR,
        OPC_WRITE_DISHES,


        OPC_READ_TOPAZ_INFO,
        OPC_READ_CAL_AB_LINEAR,
        OPC_READ_DISHES,

        OPC_READ_STATUS,

        OPC_KEEP_ALIVE,
        OPC_USE_INTERNAL_CLOCK,
        OPC_WRITE_FLASH_CHUNK,
        OPC_READ_FLASH_CHUNK,

        OPC_GET_DR,
        OPC_GET_COOKING_DATA,



        OPC_LAST_OPCODE
    }
}
