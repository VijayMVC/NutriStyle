using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class FoodPortionGramPostUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // get the execution context from the service provider temp code to makea  change
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
                logger.debug("Preset: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                

                Entity target = (Entity)pluginExecutionContext.InputParameters["Target"];

                Entity postImage = (Entity)pluginExecutionContext.PostEntityImages["postimage"];

                Entity preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];

                logger.debug("Test: testing for the initial if statement");
                logger.debug("Test: target contains dc_unit_gram_weight " + target.Contains("dc_unit_gram_weight"));
                logger.debug("Test: postImage value of dc_recipefood " + postImage.Contains("dc_recipefood"));

                if (target.Contains("dc_unit_gram_weight") && !((Boolean)postImage["dc_recipefood"]))
                {
                    logger.debug("Valid: update to the portion gram weight and the food is not a recipe.");
                    
                    double portionAdjust = (double)target["dc_unit_gram_weight"] / (double)preImage["dc_unit_gram_weight"];
                    
                    logger.debug("Checking: portion adjust of target: " + target["dc_unit_gram_weight"] + " by pre: " +
                        preImage["dc_unit_gram_weight"] + " is =" + portionAdjust);

                    // needed to reuse existing code. Will find the amount after the change of dc_unit_gram_weight and will update the entityff
                    #region nutrient variables

                    double fatSum = 0;
                    double proteinSum = 0;
                    double carbohydrateSum = 0;
                    double dc_alcoholSum = 0;
                    double dc_sugarSum = 0;
                    double dc_fa_transSum = 0;
                    double dc_cholestrolSum = 0;
                    double dc_fiberSum = 0;
                    double dc_fa_satSum = 0;
                    double dc_fa_unsatSum = 0;
                    double dc_fa_monoSum = 0;
                    double dc_fa_polySum = 0;
                    double dc_vit_cSum = 0;
                    double dc_vit_aSum = 0;
                    double dc_vit_a_iuSum = 0;
                    double dc_vit_dSum = 0;
                    decimal dc_vitamindd2d3ingSum = 0m;
                    double dc_vit_eSum = 0;
                    double dc_vit_kSum = 0;
                    double dc_thiaminSum = 0;
                    double dc_riboflavinSum = 0;
                    double dc_folate_dfeSum = 0;
                    double dc_folateSum = 0;
                    double dc_niacinSum = 0;
                    double dc_vit_b12Sum = 0;
                    double dc_panto_acidSum = 0;
                    double dc_vit_b6Sum = 0;
                    double dc_cholineSum = 0;
                    decimal dc_biotinSum = 0m;
                    decimal dc_chromiumSum = 0m;
                    double dc_calciumSum = 0;
                    double dc_potassiumSum = 0;
                    double dc_magnesiumSum = 0;
                    double dc_sodiumSum = 0;
                    double dc_phosphorusSum = 0;
                    double dc_manganeseSum = 0;
                    double dc_zincSum = 0;
                    double dc_ironSum = 0;
                    double dc_copperSum = 0;
                    double dc_seleniumSum = 0;
                    double dc_starchSum = 0;
                    double dc_sucroseSum = 0;
                    double dc_glucoseSum = 0;
                    double dc_fructoseSum = 0;
                    double dc_maltoseSum = 0;
                    double dc_galactoseSum = 0;
                    double dc_lactoseSum = 0;
                    double dc_alanineSum = 0;
                    double dc_arginineSum = 0;
                    double dc_aspartic_acidSum = 0;
                    double dc_cystineSum = 0;
                    double dc_glutamic_acidSum = 0;
                    double dc_glycineSum = 0;
                    double dc_histidineSum = 0;
                    double dc_isoleucineSum = 0;
                    double dc_leucineSum = 0;
                    double dc_lysineSum = 0;
                    double dc_methionineSum = 0;
                    double dc_phenylalanineSum = 0;
                    double dc_prolineSum = 0;
                    double dc_serineSum = 0;
                    double dc_threonineSum = 0;
                    double dc_tryptophanSum = 0;
                    double dc_tyrosineSum = 0;
                    double dc_valineSum = 0;
                    //double dc_sfa40butyricacidgSum = 0;               //single line of text not a double
                    //double dc_sfa60caproicacidgSum = 0;               //single line of text not a double
                    double dc_lipids_8_0Sum = 0;
                    double dc_lipids_10_0Sum = 0;
                    double dc_lipids_12_0Sum = 0;
                    double dc_lipids_14_0Sum = 0;
                    double dc_lipids_16_0Sum = 0;
                    double dc_lipids_18_0Sum = 0;
                    double dc_lipids_20_0Sum = 0;
                    double dc_lipids_22_0Sum = 0;
                    double dc_lipids_14_1Sum = 0;
                    double dc_lipids_16_1Sum = 0;
                    double dc_lipids_18_1Sum = 0;
                    double dc_lipids_20_1Sum = 0;
                    decimal dc_mufa221erucicacidgSum = 0m;
                    double dc_lipids_18_2Sum = 0;
                    double dc_lipids_18_3Sum = 0;
                    double dc_lipids_18_4Sum = 0;
                    double dc_lipids_20_4Sum = 0;
                    double dc_lipids_20_5_n3Sum = 0;
                    double dc_lipids_22_5_n3Sum = 0;
                    double dc_lipids_22_6_n3Sum = 0;
                    decimal dc_phytosterolsmgSum = 0m;
                    double dc_stigmasterolSum = 0;
                    double dc_campesterolSum = 0;
                    double dc_betas_sitosterolSum = 0;
                    double dc_glycemic_index_aSum = 0;
                    //double dc_glycemicindexlmhSum = 0;                //optionset not a double
                    double dc_glycemic_load_aSum = 0;
                    //double dc_glycemicloadlmhSum = 0;                 //optionset not a double
                    decimal dc_folateaddedgSum = 0m;
                    decimal dc_folateinfoodgSum = 0m;
                    double dc_retinolSum = 0;
                    double dc_carotene_alphaSum = 0;
                    double dc_carotene_betaSum = 0;
                    double dc_lycopeneSum = 0;
                    double dc_lutein_zeaxanthinSum = 0;
                    double dc_cryptoxanthin_betaSum = 0;
                    decimal dc_vitkdihydrophylloquinonegSum = 0m;
                    decimal dc_vitkmenaquinone4ingSum = 0m;
                    double dc_tocopherol_betaSum = 0;
                    double dc_tocopherol_deltaSum = 0;
                    double dc_tocopherol_gammaSum = 0;
                    decimal dc_vitamind2ergoingSum = 0m;
                    decimal dc_vitamind3choleingSum = 0m;
                    double dc_betaineSum = 0;
                    decimal dc_theobrominemgSum = 0m;
                    decimal dc_caffeinemgSum = 0m;
                    double dc_kcalsSum = 0d;

                    #endregion

                    //retrieve the dc_food_nutrients from the food from the dc_foodnutrientid on dc_food
                    #region fetch xml for the food
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name='dc_foods'>
                            <attribute name='dc_portion_amount' />
                            <attribute name='dc_portiontypeid' />
                            <attribute name='dc_unit_gram_weight' />
                            <filter type='and'>
                                <condition attribute='dc_foodsid' operator='eq' value='@ID' />
                            </filter>
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='aa'>
                                <attribute name='dc_kcals' />
                                <attribute name='dc_portion_amount' />
                                <attribute name='dc_fat' />
                                <attribute name='dc_protein' />
                                <attribute name='dc_carbohydrate' />
                                <attribute name='dc_alcohol' />
                                <attribute name='dc_sugar' />
                                <attribute name='dc_fa_trans' />
                                <attribute name='dc_cholestrol' />
                                <attribute name='dc_fiber' />
                                <attribute name='dc_fa_sat' />
                                <attribute name='dc_fa_unsat' />
                                <attribute name='dc_fa_mono' />
                                <attribute name='dc_fa_poly' />
                                <attribute name='dc_vit_c' />
                                <attribute name='dc_vit_a' />
                                <attribute name='dc_vit_a_iu' />
                                <attribute name='dc_vit_d' />
                                <attribute name='dc_vitamindd2d3ing' />
                                <attribute name='dc_vit_e' />
                                <attribute name='dc_vit_k' />
                                <attribute name='dc_thiamin' />
                                <attribute name='dc_riboflavin' />
                                <attribute name='dc_folate_dfe' />
                                <attribute name='dc_folate' />
                                <attribute name='dc_niacin' />
                                <attribute name='dc_vit_b12' />
                                <attribute name='dc_panto_acid' />
                                <attribute name='dc_vit_b6' />
                                <attribute name='dc_choline' />
                                <attribute name='dc_biotin' />
                                <attribute name='dc_chromium' />
                                <attribute name='dc_calcium' />
                                <attribute name='dc_potassium' />
                                <attribute name='dc_magnesium' />
                                <attribute name='dc_sodium' />
                                <attribute name='dc_phosphorus' />
                                <attribute name='dc_manganese' />
                                <attribute name='dc_zinc' />
                                <attribute name='dc_iron' />
                                <attribute name='dc_copper' />
                                <attribute name='dc_selenium' />
                                <attribute name='dc_starch' />
                                <attribute name='dc_sucrose' />
                                <attribute name='dc_glucose' />
                                <attribute name='dc_fructose' />
                                <attribute name='dc_maltose' />
                                <attribute name='dc_galactose' />
                                <attribute name='dc_lactose' />
                                <attribute name='dc_alanine' />
                                <attribute name='dc_arginine' />
                                <attribute name='dc_aspartic_acid' />
                                <attribute name='dc_cystine' />
                                <attribute name='dc_glutamic_acid' />
                                <attribute name='dc_glycine' />
                                <attribute name='dc_histidine' />
                                <attribute name='dc_isoleucine' />
                                <attribute name='dc_leucine' />
                                <attribute name='dc_lysine' />
                                <attribute name='dc_methionine' />
                                <attribute name='dc_phenylalanine' />
                                <attribute name='dc_proline' />
                                <attribute name='dc_serine' />
                                <attribute name='dc_threonine' />
                                <attribute name='dc_tryptophan' />
                                <attribute name='dc_tyrosine' />
                                <attribute name='dc_valine' />
                                <attribute name='dc_sfa40butyricacidg' />
                                <attribute name='dc_sfa60caproicacidg' />
                                <attribute name='dc_lipids_8_0' />
                                <attribute name='dc_lipids_10_0' />
                                <attribute name='dc_lipids_12_0' />
                                <attribute name='dc_lipids_14_0' />
                                <attribute name='dc_lipids_16_0' />
                                <attribute name='dc_lipids_18_0' />
                                <attribute name='dc_lipids_20_0' />
                                <attribute name='dc_lipids_22_0' />
                                <attribute name='dc_lipids_14_1' />
                                <attribute name='dc_lipids_16_1' />
                                <attribute name='dc_lipids_18_1' />
                                <attribute name='dc_lipids_20_1' />
                                <attribute name='dc_mufa221erucicacidg' />
                                <attribute name='dc_lipids_18_2' />
                                <attribute name='dc_lipids_18_3' />
                                <attribute name='dc_lipids_18_4' />
                                <attribute name='dc_lipids_20_4' />
                                <attribute name='dc_lipids_20_5_n3' />
                                <attribute name='dc_lipids_22_5_n3' />
                                <attribute name='dc_lipids_22_6_n3' />
                                <attribute name='dc_phytosterolsmg' />
                                <attribute name='dc_stigmasterol' />
                                <attribute name='dc_campesterol' />
                                <attribute name='dc_betas_sitosterol' />
                                <attribute name='dc_glycemic_index_a' />
                                <attribute name='dc_glycemicindexlmh' />
                                <attribute name='dc_glycemic_load_a' />
                                <attribute name='dc_glycemicloadlmh' />
                                <attribute name='dc_folateaddedg' />
                                <attribute name='dc_folateinfoodg' />
                                <attribute name='dc_retinol' />
                                <attribute name='dc_carotene_alpha' />
                                <attribute name='dc_carotene_beta' />
                                <attribute name='dc_lycopene' />
                                <attribute name='dc_lutein_zeaxanthin' />
                                <attribute name='dc_cryptoxanthin_beta' />
                                <attribute name='dc_vitkdihydrophylloquinoneg' />
                                <attribute name='dc_vitkmenaquinone4ing' />
                                <attribute name='dc_tocopherol_beta' />
                                <attribute name='dc_tocopherol_delta' />
                                <attribute name='dc_tocopherol_gamma' />
                                <attribute name='dc_vitamind2ergoing' />
                                <attribute name='dc_vitamind3choleing' />
                                <attribute name='dc_betaine' />
                                <attribute name='dc_theobrominemg' />
                                <attribute name='dc_caffeinemg' />
                            </link-entity>
                            </entity>
                        </fetch>";

                    #endregion
                    logger.debug("Checking: dc_food_nutrients id is: " + ((EntityReference)postImage["dc_foodnutrientid"]).Id);
                    Guid nutrientId = ((EntityReference)postImage["dc_foodnutrientid"]).Id;
                    logger.debug("nutrientId Guid created: ");
                    fetchXml = fetchXml.Replace("@ID", target.Id.ToString());
                    
                    EntityCollection foodResponse = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                    logger.debug("Check: if statement to enter the calculation area");
                    if (foodResponse != null)
                    {
                        logger.debug("Bad Result: dc_food_nutrients id did not return anything");
                    }
                    else
                    {
                        logger.debug("Good Result: dc_food_nutrients id retrieved a result that is not null");
                    }
                    logger.debug("Check: foodResponse count: " + foodResponse.Entities.Count());

                    #region  calculating the nutrient values
                    if (foodResponse != null && foodResponse.Entities.Count() > 0)
                    {
                        Entity foodNutrient = foodResponse.Entities[0];
                        logger.debug("Note: entered the calculation if statement. If this is not coming up then all the values will be zero");
                  

                        logger.debug("Retrieving Fat");
                        if (foodNutrient.Contains("aa.dc_fat"))
                        {
                            logger.debug("Type of dc_fat " + ((AliasedValue)foodNutrient["aa.dc_fat"]).Value.GetType());
                            double fat = foodNutrient.Contains("aa.dc_fat") ? (double)((AliasedValue)foodNutrient["aa.dc_fat"]).Value : 0;
                            logger.debug("Fat: " + fat + " food/ingredient portions: " + (fat * portionAdjust));
                            fatSum += fat * portionAdjust;
                            logger.debug("Fat Sum: " + fatSum);
                        }
                        else
                        {
                            logger.debug("there is no fat");
                        }

                        logger.debug("Retrieving protein");
                        if (foodNutrient.Contains("aa.dc_protein"))
                        {
                            double protein = foodNutrient.Contains("aa.dc_protein") ? (double)((AliasedValue)foodNutrient["aa.dc_protein"]).Value : 0;
                            logger.debug("protein: " + protein);
                            proteinSum += protein * portionAdjust;
                            logger.debug("protein Sum: " + proteinSum);
                        }
                        else
                        {
                            logger.debug("there is no protein");
                        }

                        logger.debug("Retrieving carbohydrate");
                        if (foodNutrient.Contains("aa.dc_carbohydrate"))
                        {
                            double carbohydrate = foodNutrient.Contains("aa.dc_carbohydrate") ? (double)((AliasedValue)foodNutrient["aa.dc_carbohydrate"]).Value : 0;
                            logger.debug("Carbohydrate: " + carbohydrate);
                            carbohydrateSum += carbohydrate * portionAdjust;
                            logger.debug("Carbohydrate Sum: " + carbohydrateSum);
                        }
                        else
                        {
                            logger.debug("there is no carbohydrate");
                        }

                        if (foodNutrient.Contains("aa.dc_alcohol"))
                        {
                            double alcohol = foodNutrient.Contains("aa.dc_alcohol") ? (double)((AliasedValue)foodNutrient["aa.dc_alcohol"]).Value : 0;
                            dc_alcoholSum += alcohol * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Alcohol, Ethanol");
                        }

                        if (foodNutrient.Contains("aa.dc_sugar"))
                        {
                            double sugar = foodNutrient.Contains("aa.dc_sugar") ? (double)((AliasedValue)foodNutrient["aa.dc_sugar"]).Value : 0;
                            dc_sugarSum += sugar * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Sugar, total");
                        }

                        if (foodNutrient.Contains("aa.dc_fa_trans"))
                        {
                            double fa_trans = foodNutrient.Contains("aa.dc_fa_trans") ? (double)((AliasedValue)foodNutrient["aa.dc_fa_trans"]).Value : 0;
                            dc_fa_transSum += fa_trans * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no trans Fat");
                        }

                        if (foodNutrient.Contains("aa.dc_cholestrol"))
                        {
                            double cholesterol = foodNutrient.Contains("aa.dc_cholestrol") ? (double)((AliasedValue)foodNutrient["aa.dc_cholestrol"]).Value : 0;
                            dc_cholestrolSum += cholesterol * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Cholesterol");
                        }

                        if (foodNutrient.Contains("aa.dc_fiber"))
                        {
                            double fiber = foodNutrient.Contains("aa.dc_fiber") ? (double)((AliasedValue)foodNutrient["aa.dc_fiber"]).Value : 0;
                            dc_fiberSum += fiber * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Fiber");
                        }

                        if (foodNutrient.Contains("aa.dc_fa_sat"))
                        {
                            double value = foodNutrient.Contains("aa.dc_fa_sat") ? (double)((AliasedValue)foodNutrient["aa.dc_fa_sat"]).Value : 0;
                            dc_fa_satSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Saturated Fat");
                        }

                        if (foodNutrient.Contains("aa.dc_fa_unsat"))
                        {
                            double value = foodNutrient.Contains("aa.dc_fa_unsat") ? (double)((AliasedValue)foodNutrient["aa.dc_fa_unsat"]).Value : 0;
                            dc_fa_unsatSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Unsaturated Fat");
                        }

                        if (foodNutrient.Contains("aa.dc_fa_mono"))
                        {
                            double value = foodNutrient.Contains("aa.dc_fa_mono") ? (double)((AliasedValue)foodNutrient["aa.dc_fa_mono"]).Value : 0;
                            dc_fa_monoSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Mono-unsaturated Fat");
                        }

                        if (foodNutrient.Contains("aa.dc_fa_poly"))
                        {
                            double value = foodNutrient.Contains("aa.dc_fa_poly") ? (double)((AliasedValue)foodNutrient["aa.dc_fa_poly"]).Value : 0;
                            dc_fa_polySum += value * portionAdjust;

                        }
                        else
                        {
                            logger.debug("there is no Poly-unsaturated Fat");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_c"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_c") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_c"]).Value : 0;
                            dc_vit_cSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin C");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_a"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_a") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_a"]).Value : 0;
                            dc_vit_aSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin A (RAE)");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_a_iu"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_a_iu") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_a_iu"]).Value : 0;
                            dc_vit_a_iuSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin A (IU)");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_d"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_d") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_d"]).Value : 0;
                            dc_vit_dSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin D (IU)");
                        }

                        if (foodNutrient.Contains("aa.dc_vitamindd2d3ing"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_vitamindd2d3ing") ? (decimal)((AliasedValue)foodNutrient["aa.dc_vitamindd2d3ing"]).Value : 0m;
                            dc_vitamindd2d3ingSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin D(d2+d3)");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_e"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_e") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_e"]).Value : 0;
                            dc_vit_eSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin E");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_k"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_k") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_k"]).Value : 0;
                            dc_vit_kSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin K");
                        }

                        if (foodNutrient.Contains("aa.dc_thiamin"))
                        {
                            double value = foodNutrient.Contains("aa.dc_thiamin") ? (double)((AliasedValue)foodNutrient["aa.dc_thiamin"]).Value : 0;
                            dc_thiaminSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Thiamin");
                        }

                        if (foodNutrient.Contains("aa.dc_riboflavin"))
                        {
                            double value = foodNutrient.Contains("aa.dc_riboflavin") ? (double)((AliasedValue)foodNutrient["aa.dc_riboflavin"]).Value : 0;
                            dc_riboflavinSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Riboflavin");
                        }

                        logger.debug("Retrieving Folate, total (dfe)");
                        if (foodNutrient.Contains("aa.dc_folate_dfe"))
                        {
                            double value = foodNutrient.Contains("aa.dc_folate_dfe") ? (double)((AliasedValue)foodNutrient["aa.dc_folate_dfe"]).Value : 0;
                            dc_folate_dfeSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Folate, total (dfe)");
                        }

                        if (foodNutrient.Contains("aa.dc_folate"))
                        {
                            double value = foodNutrient.Contains("aa.dc_folate") ? (double)((AliasedValue)foodNutrient["aa.dc_folate"]).Value : 0;
                            dc_folateSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Folate, total (ug)");
                        }

                        if (foodNutrient.Contains("aa.dc_niacin"))
                        {
                            double value = foodNutrient.Contains("aa.dc_niacin") ? (double)((AliasedValue)foodNutrient["aa.dc_niacin"]).Value : 0;
                            dc_niacinSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Niacin");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_b12"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_b12") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_b12"]).Value : 0;
                            dc_vit_b12Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin B12");
                        }

                        if (foodNutrient.Contains("aa.dc_panto_acid"))
                        {
                            double value = foodNutrient.Contains("aa.dc_panto_acid") ? (double)((AliasedValue)foodNutrient["aa.dc_panto_acid"]).Value : 0;
                            dc_panto_acidSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Pantothentic Acid");
                        }

                        if (foodNutrient.Contains("aa.dc_vit_b6"))
                        {
                            double value = foodNutrient.Contains("aa.dc_vit_b6") ? (double)((AliasedValue)foodNutrient["aa.dc_vit_b6"]).Value : 0;
                            dc_vit_b6Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin B6");
                        }

                        if (foodNutrient.Contains("aa.dc_choline"))
                        {
                            double value = foodNutrient.Contains("aa.dc_choline") ? (double)((AliasedValue)foodNutrient["aa.dc_choline"]).Value : 0;
                            dc_cholineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Choline");
                        }

                        if (foodNutrient.Contains("aa.dc_biotin"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_biotin") ? (decimal)((AliasedValue)foodNutrient["aa.dc_biotin"]).Value : 0m;
                            dc_biotinSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Biotin");
                        }

                        if (foodNutrient.Contains("aa.dc_chromium"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_chromium") ? (decimal)((AliasedValue)foodNutrient["aa.dc_chromium"]).Value : 0m;
                            dc_chromiumSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Chromium");
                        }

                        if (foodNutrient.Contains("aa.dc_calcium"))
                        {
                            double value = foodNutrient.Contains("aa.dc_calcium") ? (double)((AliasedValue)foodNutrient["aa.dc_calcium"]).Value : 0;
                            dc_calciumSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Cacium");
                        }

                        if (foodNutrient.Contains("aa.dc_potassium"))
                        {
                            double value = foodNutrient.Contains("aa.dc_potassium") ? (double)((AliasedValue)foodNutrient["aa.dc_potassium"]).Value : 0;
                            dc_potassiumSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Potassium");
                        }

                        if (foodNutrient.Contains("aa.dc_magnesium"))
                        {
                            double value = foodNutrient.Contains("aa.dc_magnesium") ? (double)((AliasedValue)foodNutrient["aa.dc_magnesium"]).Value : 0;
                            dc_magnesiumSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Magnesium");
                        }

                        if (foodNutrient.Contains("aa.dc_sodium"))
                        {
                            double value = foodNutrient.Contains("aa.dc_sodium") ? (double)((AliasedValue)foodNutrient["aa.dc_sodium"]).Value : 0;
                            dc_sodiumSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Sodium");
                        }

                        if (foodNutrient.Contains("aa.dc_phosphorus"))
                        {
                            double value = foodNutrient.Contains("aa.dc_phosphorus") ? (double)((AliasedValue)foodNutrient["aa.dc_phosphorus"]).Value : 0;
                            dc_phosphorusSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Phosphorus");
                        }

                        if (foodNutrient.Contains("aa.dc_manganese"))
                        {
                            double value = foodNutrient.Contains("aa.dc_manganese") ? (double)((AliasedValue)foodNutrient["aa.dc_manganese"]).Value : 0;
                            dc_manganeseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Manganese");
                        }


                        if (foodNutrient.Contains("aa.dc_zinc"))
                        {
                            double value = foodNutrient.Contains("aa.dc_zinc") ? (double)((AliasedValue)foodNutrient["aa.dc_zinc"]).Value : 0;
                            dc_zincSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Zinc");
                        }

                        if (foodNutrient.Contains("aa.dc_iron"))
                        {
                            double value = foodNutrient.Contains("aa.dc_iron") ? (double)((AliasedValue)foodNutrient["aa.dc_iron"]).Value : 0;
                            dc_ironSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Iron");
                        }

                        if (foodNutrient.Contains("aa.dc_copper"))
                        {
                            double value = foodNutrient.Contains("aa.dc_copper") ? (double)((AliasedValue)foodNutrient["aa.dc_copper"]).Value : 0;
                            dc_copperSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Copper");
                        }

                        if (foodNutrient.Contains("aa.dc_selenium"))
                        {
                            double value = foodNutrient.Contains("aa.dc_selenium") ? (double)((AliasedValue)foodNutrient["aa.dc_selenium"]).Value : 0;
                            dc_seleniumSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Selenium");
                        }

                        if (foodNutrient.Contains("aa.dc_starch"))
                        {
                            double value = foodNutrient.Contains("aa.dc_starch") ? (double)((AliasedValue)foodNutrient["aa.dc_starch"]).Value : 0;
                            dc_starchSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Starch");
                        }

                        if (foodNutrient.Contains("aa.dc_sucrose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_sucrose") ? (double)((AliasedValue)foodNutrient["aa.dc_sucrose"]).Value : 0;
                            dc_sucroseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Sucrose");
                        }

                        if (foodNutrient.Contains("aa.dc_glucose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glucose") ? (double)((AliasedValue)foodNutrient["aa.dc_glucose"]).Value : 0;
                            dc_glucoseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glucose");
                        }

                        if (foodNutrient.Contains("aa.dc_fructose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_fructose") ? (double)((AliasedValue)foodNutrient["aa.dc_fructose"]).Value : 0;
                            dc_fructoseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Fructose");
                        }

                        if (foodNutrient.Contains("aa.dc_maltose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_maltose") ? (double)((AliasedValue)foodNutrient["aa.dc_maltose"]).Value : 0;
                            dc_maltoseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Maltose");
                        }

                        if (foodNutrient.Contains("aa.dc_galactose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_galactose") ? (double)((AliasedValue)foodNutrient["aa.dc_galactose"]).Value : 0;
                            dc_galactoseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Galactose");
                        }

                        if (foodNutrient.Contains("aa.dc_lactose"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lactose") ? (double)((AliasedValue)foodNutrient["aa.dc_lactose"]).Value : 0;
                            dc_lactoseSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Lactose");
                        }

                        if (foodNutrient.Contains("aa.dc_alanine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_alanine") ? (double)((AliasedValue)foodNutrient["aa.dc_alanine"]).Value : 0;
                            dc_alanineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Alanine");
                        }

                        if (foodNutrient.Contains("aa.dc_arginine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_arginine") ? (double)((AliasedValue)foodNutrient["aa.dc_arginine"]).Value : 0;
                            dc_arginineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Arginine");
                        }

                        if (foodNutrient.Contains("aa.dc_aspartic_acid"))
                        {
                            double value = foodNutrient.Contains("aa.dc_aspartic_acid") ? (double)((AliasedValue)foodNutrient["aa.dc_aspartic_acid"]).Value : 0;
                            dc_aspartic_acidSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Aspartic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_cystine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_cystine") ? (double)((AliasedValue)foodNutrient["aa.dc_cystine"]).Value : 0;
                            dc_cystineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Cystine");
                        }

                        if (foodNutrient.Contains("aa.dc_glutamic_acid"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glutamic_acid") ? (double)((AliasedValue)foodNutrient["aa.dc_glutamic_acid"]).Value : 0;
                            dc_glutamic_acidSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glutamic Acid");
                        }

                        if (foodNutrient.Contains("aa.dc_glycine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycine") ? (double)((AliasedValue)foodNutrient["aa.dc_glycine"]).Value : 0;
                            dc_glycineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glycine");
                        }

                        if (foodNutrient.Contains("aa.dc_histidine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_histidine") ? (double)((AliasedValue)foodNutrient["aa.dc_histidine"]).Value : 0;
                            dc_histidineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Histidine");
                        }

                        if (foodNutrient.Contains("aa.dc_isoleucine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_isoleucine") ? (double)((AliasedValue)foodNutrient["aa.dc_isoleucine"]).Value : 0;
                            dc_isoleucineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Isoleucine");
                        }

                        if (foodNutrient.Contains("aa.dc_leucine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_leucine") ? (double)((AliasedValue)foodNutrient["aa.dc_leucine"]).Value : 0;
                            dc_leucineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Leucine");
                        }

                        if (foodNutrient.Contains("aa.dc_lysine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lysine") ? (double)((AliasedValue)foodNutrient["aa.dc_lysine"]).Value : 0;
                            dc_lysineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Lysine");
                        }

                        if (foodNutrient.Contains("aa.dc_methionine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_methionine") ? (double)((AliasedValue)foodNutrient["aa.dc_methionine"]).Value : 0;
                            dc_methionineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Methionine");
                        }

                        if (foodNutrient.Contains("aa.dc_phenylalanine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_phenylalanine") ? (double)((AliasedValue)foodNutrient["aa.dc_phenylalanine"]).Value : 0;
                            dc_phenylalanineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Phenylalanine");
                        }

                        if (foodNutrient.Contains("aa.dc_proline"))
                        {
                            double value = foodNutrient.Contains("aa.dc_proline") ? (double)((AliasedValue)foodNutrient["aa.dc_proline"]).Value : 0;
                            dc_prolineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Proline");
                        }

                        if (foodNutrient.Contains("aa.dc_serine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_serine") ? (double)((AliasedValue)foodNutrient["aa.dc_serine"]).Value : 0;
                            dc_serineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Serine");
                        }

                        if (foodNutrient.Contains("aa.dc_threonine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_threonine") ? (double)((AliasedValue)foodNutrient["aa.dc_threonine"]).Value : 0;
                            dc_threonineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Threonine");
                        }

                        if (foodNutrient.Contains("aa.dc_tryptophan"))
                        {
                            double value = foodNutrient.Contains("aa.dc_tryptophan") ? (double)((AliasedValue)foodNutrient["aa.dc_tryptophan"]).Value : 0;
                            dc_tryptophanSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Typtophan");
                        }

                        if (foodNutrient.Contains("aa.dc_tyrosine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_tyrosine") ? (double)((AliasedValue)foodNutrient["aa.dc_tyrosine"]).Value : 0;
                            dc_tyrosineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Tyrosine");
                        }

                        if (foodNutrient.Contains("aa.dc_valine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_valine") ? (double)((AliasedValue)foodNutrient["aa.dc_valine"]).Value : 0;
                            dc_valineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Valine");
                        }

                        #region Unused Code
                        /*logger.debug("Retrieving SFA 4:0 (butyric acid)");       //string, if wanted needs different logic
                        if (foodNutrient.Contains("aa.dc_sfa40butyricacidg"))
                        {
                            double value = foodNutrient.Contains("aa.dc_sfa40butyricacidg") ? (double)((AliasedValue)foodNutrient["aa.dc_sfa40butyricacidg"]).Value : 0;
                            logger.debug("SFA 4:0 (butyric acid): " + value);
                            dc_sfa40butyricacidgSum += value;
                            logger.debug("SFA 4:0 (butyric acid) Sum: " + dc_sfa40butyricacidgSum);
                        }
                        else
                        {
                            logger.debug("there is no SFA 4:0 (butyric acid)");
                        }

                        logger.debug("Retrieving SFA 6:0 (Caproicacid)");          //string, if wanted needs different logic
                        if (foodNutrient.Contains("aa.dc_sfa60caproicacidg"))
                        {
                            double value = foodNutrient.Contains("aa.dc_sfa60caproicacidg") ? (double)((AliasedValue)foodNutrient["aa.dc_sfa60caproicacidg"]).Value : 0;
                            logger.debug("SFA 6:0 (Caproicacid): " + value);
                            dc_sfa60caproicacidgSum += value;
                            logger.debug("SFA 6:0 (Caproicacid) Sum: " + dc_sfa60caproicacidgSum);
                        }
                        else
                        {
                            logger.debug("there is no SFA 6:0 (Caproicacid)");
                        }*/
                        
                        #endregion

                        if (foodNutrient.Contains("aa.dc_lipids_8_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_8_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_8_0"]).Value : 0;
                            dc_lipids_8_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no SFA 8:0 caprylicacid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_10_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_10_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_10_0"]).Value : 0;
                            dc_lipids_10_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 10:0 capric acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_12_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_12_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_12_0"]).Value : 0;
                            dc_lipids_12_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 12:0 lauric acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_14_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_14_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_14_0"]).Value : 0;
                            dc_lipids_14_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 14:0 myristic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_16_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_16_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_16_0"]).Value : 0;
                            dc_lipids_16_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 16:0 palmitic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_18_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_18_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_18_0"]).Value : 0;
                            dc_lipids_18_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 18:0 stearic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_20_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_20_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_20_0"]).Value : 0;
                            dc_lipids_20_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 20:0 arachidicacid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_22_0"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_22_0") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_22_0"]).Value : 0;
                            dc_lipids_22_0Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no sfa 22:0 hehenic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_14_1"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_14_1") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_14_1"]).Value : 0;
                            dc_lipids_14_1Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no mufa 14:1 myristoleic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_16_1"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_16_1") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_16_1"]).Value : 0;
                            dc_lipids_16_1Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no mufa 16:1 palmitoleic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_18_1"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_18_1") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_18_1"]).Value : 0;
                            dc_lipids_18_1Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no mufa 18:1 oleic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_20_1"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_20_1") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_20_1"]).Value : 0;
                            dc_lipids_20_1Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no mufa 20:1 gadoleic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_mufa221erucicacidg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_mufa221erucicacidg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_mufa221erucicacidg"]).Value : 0m;
                            dc_mufa221erucicacidgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no mufa 22:1 erucic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_18_2"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_18_2") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_18_2"]).Value : 0;
                            dc_lipids_18_2Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 18:2 linoleic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_18_3"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_18_3") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_18_3"]).Value : 0;
                            dc_lipids_18_3Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 18:3 linolenic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_18_4"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_18_4") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_18_4"]).Value : 0;
                            dc_lipids_18_4Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 18:4 parinaric acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_20_4"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_20_4") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_20_4"]).Value : 0;
                            dc_lipids_20_4Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 20:4 arachidonic acid");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_20_5_n3"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_20_5_n3") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_20_5_n3"]).Value : 0;
                            dc_lipids_20_5_n3Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 20:5 EPA");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_22_5_n3"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_22_5_n3") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_22_5_n3"]).Value : 0;
                            dc_lipids_22_5_n3Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 22:5 DPA");
                        }

                        if (foodNutrient.Contains("aa.dc_lipids_22_6_n3"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lipids_22_6_n3") ? (double)((AliasedValue)foodNutrient["aa.dc_lipids_22_6_n3"]).Value : 0;
                            dc_lipids_22_6_n3Sum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pufa 22:6 DHA");
                        }

                        if (foodNutrient.Contains("aa.dc_phytosterolsmg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_phytosterolsmg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_phytosterolsmg"]).Value : 0m;
                            dc_phytosterolsmgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no pytosterols");
                        }

                        if (foodNutrient.Contains("aa.dc_stigmasterol"))
                        {
                            double value = foodNutrient.Contains("aa.dc_stigmasterol") ? (double)((AliasedValue)foodNutrient["aa.dc_stigmasterol"]).Value : 0;
                            dc_stigmasterolSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Stigmasterol");
                        }

                        if (foodNutrient.Contains("aa.dc_campesterol"))
                        {
                            double value = foodNutrient.Contains("aa.dc_campesterol") ? (double)((AliasedValue)foodNutrient["aa.dc_campesterol"]).Value : 0;
                            dc_campesterolSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Campesterol");
                        }

                        if (foodNutrient.Contains("aa.dc_betas_sitosterol"))
                        {
                            double value = foodNutrient.Contains("aa.dc_betas_sitosterol") ? (double)((AliasedValue)foodNutrient["aa.dc_betas_sitosterol"]).Value : 0;
                            dc_betas_sitosterolSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Beta-sitosterol");
                        }

                        if (foodNutrient.Contains("aa.dc_glycemic_index_a"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycemic_index_a") ? (double)((AliasedValue)foodNutrient["aa.dc_glycemic_index_a"]).Value : 0;
                            dc_glycemic_index_aSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glycemic Index #");
                        }

                        #region Unused Code
                        /*logger.debug("Retrieving Glycemic Index L, M, H");           //optionset, if wanted needs different logic
                        if (foodNutrient.Contains("aa.dc_glycemicindexlmh"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycemicindexlmh") ? (double)((AliasedValue)foodNutrient["aa.dc_glycemicindexlmh"]).Value : 0;
                            logger.debug("Glycemic Index L, M, H: " + value);
                            dc_glycemicindexlmhSum += value;
                            logger.debug("Glycemic Index L, M, H Sum: " + dc_glycemicindexlmhSum);
                        }
                        else
                        {
                            logger.debug("there is no Glycemic Index L, M, H");
                        }*/
                        
                        #endregion

                        if (foodNutrient.Contains("aa.dc_glycemic_load_a"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycemic_load_a") ? (double)((AliasedValue)foodNutrient["aa.dc_glycemic_load_a"]).Value : 0;
                            dc_glycemic_load_aSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glycemic load");
                        }

                        #region Unused Code
                        /*logger.debug("Retrieving Glycemic Load L, M, H");            //optionset, if wanted needs different logic
                        if (foodNutrient.Contains("aa.dc_glycemicloadlmh"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycemicloadlmh") ? (double)((AliasedValue)foodNutrient["aa.dc_glycemicloadlmh"]).Value : 0;
                            logger.debug("Glycemic Load L, M, H: " + value);
                            dc_glycemicloadlmhSum += value;
                            logger.debug("Glycemic Load L, M, H Sum: " + dc_glycemicloadlmhSum);
                        }
                        else
                        {
                            logger.debug("there is no Glycemic Load L, M, H");
                        }*/
                        
                        #endregion

                        if (foodNutrient.Contains("aa.dc_folateaddedg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_folateaddedg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_folateaddedg"]).Value : 0m;
                            dc_folateaddedgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Folate, added");
                        }

                        if (foodNutrient.Contains("aa.dc_folateinfoodg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_folateinfoodg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_folateinfoodg"]).Value : 0m;
                            dc_folateinfoodgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Folate, in food");
                        }

                        if (foodNutrient.Contains("aa.dc_retinol"))
                        {
                            double value = foodNutrient.Contains("aa.dc_retinol") ? (double)((AliasedValue)foodNutrient["aa.dc_retinol"]).Value : 0;
                            dc_retinolSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Retinol");
                        }

                        if (foodNutrient.Contains("aa.dc_carotene_alpha"))
                        {
                            double value = foodNutrient.Contains("aa.dc_carotene_alpha") ? (double)((AliasedValue)foodNutrient["aa.dc_carotene_alpha"]).Value : 0;
                            dc_carotene_alphaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Carotene, alpha");
                        }

                        if (foodNutrient.Contains("aa.dc_carotene_beta"))
                        {
                            double value = foodNutrient.Contains("aa.dc_carotene_beta") ? (double)((AliasedValue)foodNutrient["aa.dc_carotene_beta"]).Value : 0;
                            dc_carotene_betaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Carotene, beta");
                        }

                        if (foodNutrient.Contains("aa.dc_lycopene"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lycopene") ? (double)((AliasedValue)foodNutrient["aa.dc_lycopene"]).Value : 0;
                            dc_lycopeneSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Lycopene");
                        }

                        if (foodNutrient.Contains("aa.dc_lutein_zeaxanthin"))
                        {
                            double value = foodNutrient.Contains("aa.dc_lutein_zeaxanthin") ? (double)((AliasedValue)foodNutrient["aa.dc_lutein_zeaxanthin"]).Value : 0;
                            dc_lutein_zeaxanthinSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Lutein + Zeaxanthin");
                        }

                        if (foodNutrient.Contains("aa.dc_cryptoxanthin_beta"))
                        {
                            double value = foodNutrient.Contains("aa.dc_cryptoxanthin_beta") ? (double)((AliasedValue)foodNutrient["aa.dc_cryptoxanthin_beta"]).Value : 0;
                            dc_cryptoxanthin_betaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Cyptoxanthin, beta");
                        }

                        if (foodNutrient.Contains("aa.dc_vitkdihydrophylloquinoneg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_vitkdihydrophylloquinoneg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_vitkdihydrophylloquinoneg"]).Value : 0m;
                            dc_vitkdihydrophylloquinonegSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vit K DH");
                        }

                        if (foodNutrient.Contains("aa.dc_vitkmenaquinone4ing"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_vitkmenaquinone4ing") ? (decimal)((AliasedValue)foodNutrient["aa.dc_vitkmenaquinone4ing"]).Value : 0m;
                            dc_vitkmenaquinone4ingSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vit K M");
                        }

                        if (foodNutrient.Contains("aa.dc_tocopherol_beta"))
                        {
                            double value = foodNutrient.Contains("aa.dc_tocopherol_beta") ? (double)((AliasedValue)foodNutrient["aa.dc_tocopherol_beta"]).Value : 0;
                            dc_tocopherol_betaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Tocopherol, beta");
                        }

                        if (foodNutrient.Contains("aa.dc_tocopherol_delta"))
                        {
                            double value = foodNutrient.Contains("aa.dc_tocopherol_delta") ? (double)((AliasedValue)foodNutrient["aa.dc_tocopherol_delta"]).Value : 0;
                            dc_tocopherol_deltaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Tocopherol, delta");
                        }

                        if (foodNutrient.Contains("aa.dc_tocopherol_gamma"))
                        {
                            double value = foodNutrient.Contains("aa.dc_tocopherol_gamma") ? (double)((AliasedValue)foodNutrient["aa.dc_tocopherol_gamma"]).Value : 0;
                            dc_tocopherol_gammaSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Tocopherol, gamma");
                        }

                        if (foodNutrient.Contains("aa.dc_vitamind2ergoing"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_vitamind2ergoing") ? (decimal)((AliasedValue)foodNutrient["aa.dc_vitamind2ergoing"]).Value : 0m;
                            dc_vitamind2ergoingSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin D2");
                        }

                        if (foodNutrient.Contains("aa.dc_vitamind3choleing"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_vitamind3choleing") ? (decimal)((AliasedValue)foodNutrient["aa.dc_vitamind3choleing"]).Value : 0m;
                            dc_vitamind3choleingSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Vitamin D3");
                        }

                        if (foodNutrient.Contains("aa.dc_betaine"))
                        {
                            double value = foodNutrient.Contains("aa.dc_betaine") ? (double)((AliasedValue)foodNutrient["aa.dc_betaine"]).Value : 0;
                            dc_betaineSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Betaine");
                        }

                        if (foodNutrient.Contains("aa.dc_theobrominemg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_theobrominemg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_theobrominemg"]).Value : 0m;
                            dc_theobrominemgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Theobromine");
                        }

                        if (foodNutrient.Contains("aa.dc_caffeinemg"))
                        {
                            decimal value = foodNutrient.Contains("aa.dc_caffeinemg") ? (decimal)((AliasedValue)foodNutrient["aa.dc_caffeinemg"]).Value : 0m;
                            dc_caffeinemgSum += value * (decimal)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Caffeine");
                        }

                        if (foodNutrient.Contains("aa.dc_kcals"))
                        {
                            double value = foodNutrient.Contains("aa.dc_kcals") ? (double)((AliasedValue)foodNutrient["aa.dc_kcals"]).Value : 0d;
                            dc_kcalsSum += value * (double)portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no kcals");
                        }


                    }
                    #endregion

                    //build a dc_food_nutrients entity

                    #region Building the nutrients in an entity
                    //build nutrient information
                    Entity nutrient = new Entity("dc_food_nutrients");
                    logger.debug("Update: Entity to build dc_food_nutrients is being built");
                    nutrient["dc_fat"] = fatSum;
                    nutrient["dc_carbohydrate"] = carbohydrateSum;
                    nutrient["dc_protein"] = proteinSum;
                    nutrient["dc_alcohol"] = dc_alcoholSum;
                    nutrient["dc_sugar"] = dc_sugarSum;
                    nutrient["dc_fa_trans"] = dc_fa_transSum;
                    nutrient["dc_cholestrol"] = dc_cholestrolSum;
                    nutrient["dc_fiber"] = dc_fiberSum;
                    nutrient["dc_fa_sat"] = dc_fa_satSum;
                    nutrient["dc_fa_unsat"] = dc_fa_unsatSum;
                    nutrient["dc_fa_mono"] = dc_fa_monoSum;
                    nutrient["dc_fa_poly"] = dc_fa_polySum;
                    nutrient["dc_vit_c"] = dc_vit_cSum;
                    nutrient["dc_vit_a"] = dc_vit_aSum;
                    nutrient["dc_vit_a_iu"] = dc_vit_a_iuSum;
                    nutrient["dc_vit_d"] = dc_vit_dSum;
                    nutrient["dc_vitamindd2d3ing"] = dc_vitamindd2d3ingSum;
                    nutrient["dc_vit_e"] = dc_vit_eSum;
                    nutrient["dc_vit_k"] = dc_vit_kSum;
                    nutrient["dc_thiamin"] = dc_thiaminSum;
                    nutrient["dc_riboflavin"] = dc_riboflavinSum;
                    nutrient["dc_folate_dfe"] = dc_folate_dfeSum;
                    nutrient["dc_folate"] = dc_folateSum;
                    nutrient["dc_niacin"] = dc_niacinSum;
                    nutrient["dc_vit_b12"] = dc_vit_b12Sum;
                    nutrient["dc_panto_acid"] = dc_panto_acidSum;
                    nutrient["dc_vit_b6"] = dc_vit_b6Sum;
                    nutrient["dc_choline"] = dc_cholineSum;
                    nutrient["dc_biotin"] = dc_biotinSum;
                    nutrient["dc_chromium"] = dc_chromiumSum;
                    nutrient["dc_calcium"] = dc_calciumSum;
                    nutrient["dc_potassium"] = dc_potassiumSum;
                    nutrient["dc_magnesium"] = dc_magnesiumSum;
                    nutrient["dc_sodium"] = dc_sodiumSum;
                    nutrient["dc_phosphorus"] = dc_phosphorusSum;
                    nutrient["dc_manganese"] = dc_manganeseSum;
                    nutrient["dc_zinc"] = dc_zincSum;
                    nutrient["dc_iron"] = dc_ironSum;
                    nutrient["dc_copper"] = dc_copperSum;
                    nutrient["dc_selenium"] = dc_seleniumSum;
                    nutrient["dc_starch"] = dc_starchSum;
                    nutrient["dc_sucrose"] = dc_sucroseSum;
                    nutrient["dc_glucose"] = dc_glucoseSum;
                    nutrient["dc_fructose"] = dc_fructoseSum;
                    nutrient["dc_maltose"] = dc_maltoseSum;
                    nutrient["dc_galactose"] = dc_galactoseSum;
                    nutrient["dc_lactose"] = dc_lactoseSum;
                    nutrient["dc_alanine"] = dc_alanineSum;
                    nutrient["dc_arginine"] = dc_arginineSum;
                    nutrient["dc_aspartic_acid"] = dc_aspartic_acidSum;
                    nutrient["dc_cystine"] = dc_cystineSum;
                    nutrient["dc_glutamic_acid"] = dc_glutamic_acidSum;
                    nutrient["dc_glycine"] = dc_glycineSum;
                    nutrient["dc_histidine"] = dc_histidineSum;
                    nutrient["dc_isoleucine"] = dc_isoleucineSum;
                    nutrient["dc_leucine"] = dc_leucineSum;
                    nutrient["dc_lysine"] = dc_lysineSum;
                    nutrient["dc_methionine"] = dc_methionineSum;
                    nutrient["dc_phenylalanine"] = dc_phenylalanineSum;
                    nutrient["dc_proline"] = dc_prolineSum;
                    nutrient["dc_serine"] = dc_serineSum;
                    nutrient["dc_threonine"] = dc_threonineSum;
                    nutrient["dc_tryptophan"] = dc_tryptophanSum;
                    nutrient["dc_tyrosine"] = dc_tyrosineSum;
                    nutrient["dc_valine"] = dc_valineSum;
                    //nutrient["dc_sfa40butyricacidg"] = dc_sfa40butyricacidgSum.ToString();        //single line of text not a double or decimal
                    //nutrient["dc_sfa60caproicacidg"] = dc_sfa60caproicacidgSum;                   //single line of text not a double or decimal
                    nutrient["dc_lipids_8_0"] = dc_lipids_8_0Sum;
                    nutrient["dc_lipids_10_0"] = dc_lipids_10_0Sum;
                    nutrient["dc_lipids_12_0"] = dc_lipids_12_0Sum;
                    nutrient["dc_lipids_14_0"] = dc_lipids_14_0Sum;
                    nutrient["dc_lipids_16_0"] = dc_lipids_16_0Sum;
                    nutrient["dc_lipids_18_0"] = dc_lipids_18_0Sum;
                    nutrient["dc_lipids_20_0"] = dc_lipids_20_0Sum;
                    nutrient["dc_lipids_22_0"] = dc_lipids_22_0Sum;
                    nutrient["dc_lipids_14_1"] = dc_lipids_14_1Sum;
                    nutrient["dc_lipids_16_1"] = dc_lipids_16_1Sum;
                    nutrient["dc_lipids_18_1"] = dc_lipids_18_1Sum;
                    nutrient["dc_lipids_20_1"] = dc_lipids_20_1Sum;
                    nutrient["dc_mufa221erucicacidg"] = dc_mufa221erucicacidgSum;
                    nutrient["dc_lipids_18_2"] = dc_lipids_18_2Sum;
                    nutrient["dc_lipids_18_3"] = dc_lipids_18_3Sum;
                    nutrient["dc_lipids_18_4"] = dc_lipids_18_4Sum;
                    nutrient["dc_lipids_20_4"] = dc_lipids_20_4Sum;
                    nutrient["dc_lipids_20_5_n3"] = dc_lipids_20_5_n3Sum;
                    nutrient["dc_lipids_22_5_n3"] = dc_lipids_22_5_n3Sum;
                    nutrient["dc_lipids_22_6_n3"] = dc_lipids_22_6_n3Sum;
                    nutrient["dc_phytosterolsmg"] = dc_phytosterolsmgSum;
                    nutrient["dc_stigmasterol"] = dc_stigmasterolSum;
                    nutrient["dc_campesterol"] = dc_campesterolSum;
                    nutrient["dc_betas_sitosterol"] = dc_betas_sitosterolSum;
                    nutrient["dc_glycemic_index_a"] = dc_glycemic_index_aSum;
                    //nutrient["dc_glycemicindexlmh"] = dc_glycemicindexlmhSum;         //optionset
                    nutrient["dc_glycemic_load_a"] = dc_glycemic_load_aSum;
                    //nutrient["dc_glycemicloadlmh"] = dc_glycemicloadlmhSum;             //optionset
                    nutrient["dc_folateaddedg"] = dc_folateaddedgSum;
                    nutrient["dc_folateinfoodg"] = dc_folateinfoodgSum;
                    nutrient["dc_retinol"] = dc_retinolSum;
                    nutrient["dc_carotene_alpha"] = dc_carotene_alphaSum;
                    nutrient["dc_carotene_beta"] = dc_carotene_betaSum;
                    nutrient["dc_lycopene"] = dc_lycopeneSum;
                    nutrient["dc_lutein_zeaxanthin"] = dc_lutein_zeaxanthinSum;
                    nutrient["dc_cryptoxanthin_beta"] = dc_cryptoxanthin_betaSum;
                    nutrient["dc_vitkdihydrophylloquinoneg"] = dc_vitkdihydrophylloquinonegSum;
                    nutrient["dc_vitkmenaquinone4ing"] = dc_vitkmenaquinone4ingSum;
                    nutrient["dc_tocopherol_beta"] = dc_tocopherol_betaSum;
                    nutrient["dc_tocopherol_delta"] = dc_tocopherol_deltaSum;
                    nutrient["dc_tocopherol_gamma"] = dc_tocopherol_gammaSum;
                    nutrient["dc_vitamind2ergoing"] = dc_vitamind2ergoingSum;
                    nutrient["dc_vitamind3choleing"] = dc_vitamind3choleingSum;
                    nutrient["dc_betaine"] = dc_betaineSum;
                    nutrient["dc_theobrominemg"] = dc_theobrominemgSum;
                    nutrient["dc_caffeinemg"] = dc_caffeinemgSum;
                    nutrient["dc_kcals"] = dc_kcalsSum;

                    #endregion
                    logger.debug("Update: Entity to build dc_food_nutrients, all values are set");

                    nutrient["dc_food_nutrientsid"] = nutrientId;
                    logger.debug("nutrient dc_food_nutrientsid assigned");
                    logger.debug("nutrient information updated");
                    crmService.Update(nutrient);

                    logger.debug("Completed: FoodPortionGramPostUpdate Nutrient update");
                    

                }

                
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
    }
}
