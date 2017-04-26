using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers
{
    class ConversionFactor
    {
        private string parentPortion;
        private string ingredientPortion;

        public ConversionFactor(Entity parent, Entity ingredient)
        {
            parentPortion = ((EntityReference)parent["dc_portiontypeid"]).Name;
            ingredientPortion = ((EntityReference)ingredient["dc_portiontypeid"]).Name;
        }

        /*
         * will determine the conversion factor based on converting the partentPortion 
         * to the same portion type as the ingredient portion
         * Example ingredientPortion is in cups and parentPortion is ounces. the conversion is
         * ounces into cups so the facter will returned be 1/8
         */
        public double getConversionFactor()
        {
            if (ingredientPortion == "cup (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return (1.0 / 8.0);
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 48.0);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 16.0);
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return 2.0;
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 4.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 16.0;
                }
                else if ("milliliter" == parentPortion)
                {
                    return (1.0 / 240.0);
                }
            }
            else if (ingredientPortion == "cup (dry)")
            {
                if ("ounce (dry)" == parentPortion)
                {
                    return (1.0 / 8.0);
                }
                else if ("teaspoon (dry)" == parentPortion)
                {
                    return (1.0 / 48.0);
                }
                else if ("tablespoon (dry)" == parentPortion)
                {
                    return (1.0 / 16.0);
                }
                if ("pound" == parentPortion)
                {
                    return 2.0;
                }
            }
            else if (ingredientPortion == "ounce (liquid)" || ingredientPortion == "fluid ounce")
            {
                if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 6.0);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 2.0);
                }
                else if ("cup (liquid)" == parentPortion)
                {
                    return 8.0;
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return 16.0;
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 32.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 128.0;
                }
                else if ("milliliter" == parentPortion)
                {
                    return (1.0 / 30.0);
                }
            }
            else if (ingredientPortion == "ounce (dry)")
            {
                if ("teaspoon (dry)" == parentPortion)
                {
                    return (1.0 / 6.0);
                }
                else if ("tablespoon (dry)" == parentPortion)
                {
                    return 2;
                }
                if ("cup (dry)" == parentPortion)
                {
                    return 8.0;
                }
                if ("pound" == parentPortion)
                {
                    return 16.0;
                }
            }
            else if (ingredientPortion == "teaspoon (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return 6.0;
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return 3.0;
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return 48.0;
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return 96.0;
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 192.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 768.0;
                }
                else if ("milliliter" == parentPortion)
                {
                    return (1.0 / 5.0);
                }
            }
            else if (ingredientPortion == "teaspoon (dry)")
            {
                if ("ounce (dry)" == parentPortion)
                {
                    return 6.0;
                }
                else if ("tablespoon (dry)" == parentPortion)
                {
                    return 3.0;
                }
                if ("cup (dry)" == parentPortion)
                {
                    return 48.0;
                }
                if ("pound" == parentPortion)
                {
                    return 96.0;
                }
            }
            else if (ingredientPortion == "tablespoon (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return 2;
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 3.0);
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return 16.0;
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return 32.0;
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 64.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 256.0;
                }
                else if ("milliliter" == parentPortion)
                {
                    return (1.0 / 15.0);
                }
            }
            else if (ingredientPortion == "tablespoon (dry)")
            {
                if ("ounce (dry)" == parentPortion)
                {
                    return 2.0;
                }
                else if ("teaspoon (dry)" == parentPortion)
                {
                    return (1.0 / 3.0);
                }
                if ("cup (dry)" == parentPortion)
                {
                    return 16.0;
                }
                if ("pound" == parentPortion)
                {
                    return 32.0;
                }
            }
            else if (ingredientPortion == "pint (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return (1.0 / 16.0);
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 96.0);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 32.0);
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return 1.0 / 2.0; 
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 2.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 8.0;
                }
            }
            else if (ingredientPortion == "quart (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return (1.0 / 32.0);
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 192.0);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 64.0);
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return (1.0 / 4.0);
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return (1.0 / 2.0);
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 4.0;
                }
                else if ("liter)" == parentPortion)
                {
                    return (1.0 / 0.946);
                }
            }
            else if (ingredientPortion == "gallon (liquid)")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return (1.0 / 128.0);
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 768.0);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 256.0);
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return (1.0 / 16.0);
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return (1.0 / 8.0);
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return (1.0 / 4.0);
                }
                else if ("liter)" == parentPortion)
                {
                    return (1.0 / 3.785);
                }
            }
            else if (ingredientPortion == "pound")
            {
                if ("teaspoon (dry)" == parentPortion)
                {
                    return (1.0 / 96.0);
                }
                else if ("tablespoon (dry))" == parentPortion)
                {
                    return (1.0 / 32.0);
                }
                else if ("ounce (dry))" == parentPortion)
                {
                    return (1.0 / 16.0);
                }
                else if ("cup (dry))" == parentPortion)
                {
                    return (1.0 / 2.0);
                }
                else if ("kilogram)" == parentPortion)
                {
                    return 2.205;
                }
            }
            else if (ingredientPortion == "kilogram")
            {
                if ("pound)" == parentPortion)
                {
                    return (1.0 / 2.205);
                }
            }
            else if (ingredientPortion == "milliliter")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return 30.0;
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return 5.0;
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return 15.0;
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return 240.0;
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return 473.176;
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return 946.352;
                }
                else if ("liter)" == parentPortion)
                {
                    return 1000.0;
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 3785.411;
                }
            }
            else if (ingredientPortion == "liter")
            {
                if ("ounce (liquid)" == parentPortion || "fluid ounce" == parentPortion)
                {
                    return (1.0 / 33.814);
                }
                else if ("teaspoon (liquid)" == parentPortion)
                {
                    return (1.0 / 202.884);
                }
                else if ("tablespoon (liquid)" == parentPortion)
                {
                    return (1.0 / 67.628);
                }
                if ("cup (liquid)" == parentPortion)
                {
                    return (1.0 / 4.226);
                }
                else if ("pint (liquid)" == parentPortion)
                {
                    return (1.0 / 2.113);
                }
                else if ("quart (liquid)" == parentPortion)
                {
                    return (1.0 / 1.056);
                }
                else if ("milliliter)" == parentPortion)
                {
                    return (1.0 / 1000.0);
                }
                else if ("gallon (liquid)" == parentPortion)
                {
                    return 3.785;
                }
            }

            return 1.0;
        }
    }
}
