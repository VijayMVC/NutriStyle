﻿using System;
using System.Collections.Generic;
using System.Configuration;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
//using System.ServiceModel;

namespace MenuGeneratorTest
{
    public class Program
    {
        static void Main(string[] args)
        {                    
            
            //Caleb
            OrganizationServiceProxy crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NSDEV", 443);
            //OrganizationServiceProxy crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NSDEV", 443);
            //Guid contactId = new Guid("F958A7DC-3807-E111-9777-00155D0A0205"); //-- DEV
            Guid contactId = new Guid("F958A7DC-3807-E111-9777-00155D0A0205");//655D50ED-71A0-E111-9BC3-00155D0A0C06 -- PROD//Scott's user: 9AA409B6-55A0-E111-9BC3-00155D0A0C06
            BuildMenu bm = new BuildMenu(crmService);
            bm.Build(contactId);
            
            //retrieve presets
            int kcalBase = 1800;
            EntityCollection presets = CrmHelper.GetEntitiesByAttribute("dc_presets", "statecode", 0, new String[] { "dc_presetsid", "dc_name" }, null, crmService);

            foreach (Entity preset in presets.Entities)
            {
                System.Console.WriteLine("Preset name: "+(String)preset["dc_name"]);
                if (((String)preset["dc_name"]).Equals("3 Apple-a-Day"))
                {
                    Entity contact = new Entity("contact");
                    contact.Id = contactId;
                    contact["dc_menupresetid"] = new EntityReference("dc_presets", preset.Id);
                    crmService.Update(contact);

                    for (int x = 0; x < 1; x++)
                    {
                        System.Console.WriteLine("Kcal target: " + Convert.ToDecimal(kcalBase + (x * 100)));
                        contact = new Entity("contact");
                        contact.Id = contactId;
                        contact["dc_userspecifiedkcaltarget"] = true; //yes
                        contact["dc_kcaltarget"] = Convert.ToDecimal(kcalBase + (x * 100));
                        crmService.Update(contact);
                        bm.Build(contactId);
                    }
                   
                }
                break;
            }
            

        }
    }
}
