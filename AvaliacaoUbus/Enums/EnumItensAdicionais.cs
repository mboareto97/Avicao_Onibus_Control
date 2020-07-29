using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvaliacaoUbus.Enums
{
    public enum EnumItensAdicionais
    {
        [Description("Wi-Fi")]
        WiFi = 1,

        [Description("Banheiro")]
        Banheiro,

        [Description("Ar Condicionado")]
        ArCondicionado,

        [Description("TV")]
        TV,

        [Description("Frigobar")]
        Frigobar
    }
}
