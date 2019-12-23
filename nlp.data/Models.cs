using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public static class Models
    {
        public static IModel<Model> Vanguard =>
            new Model()
            {
                Id = "984ce69d-de79-478b-9223-ff6349514e19",
                Name = "Vanguard",
                Children = {
                    new Model()
                    {
                        Id = "5ec6957d-4de7-4199-9373-d4a7fb59d6e1",
                        Name = "Index Funds",
                        Details = "vbiix|vbinx|vbisx|vbltx|vbmfx|vdaix|vdvix|veiex|veurx|vexmx|vfinx|vfsvx|vftsx|vfwix|vgovx|vgtsx|vhdyx|viaix|vigrx|vihix|vimsx|visgx|visvx|vivax|vlacx|vmgix|vmvix|vpacx|vtebx|vtibx|vtipx|vtsax|vtsmx|vtws"
                    }
                }
            };
    }
}
