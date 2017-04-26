using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers
{
    public class Ingredient
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        public double Rollup(IOrganizationService crmService, Guid parentFoodId, double numberOfServings, double portionSize, bool updateParent)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);

            decimal kcalsSum = 0m;
            double unitGramWeightSum = 0d;
            double unitGramWeightMultipler = 0d;

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

            #endregion

            if (parentFoodId != Guid.Empty)
            {
                //Find child collection
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_ingredient'>
                        <attribute name='dc_ingredientid' />
                        <attribute name='dc_foodingredientid' />
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_name' />
                        <attribute name='dc_originalgramweight' />
                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='aa'>
                          <filter type='and'>
                            <condition attribute='dc_foodsid' operator='eq' value='@ID' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";
                fetchXml = fetchXml.Replace("@ID", parentFoodId.ToString());

                EntityCollection response = null;
                try
                {
                    response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                }
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                {
                    logger.debug("Code: " + ex.Detail.ErrorCode);
                    logger.debug("Message: " + ex.Detail.Message);
                    logger.debug("Trace: " + ex.Detail.TraceText);
                    logger.debug("Inner Fault: " + ex.Detail.InnerFault);
                }
                logger.debug("Records found: " + response.Entities.Count);

                string buildRecipe = null;
                double amount = 0.0;
                string portionType = null;
                string ingredientName = null;
                double ingredientGrams = 0.0;

                //loop through each ingredient and find it's micro and macro nutrients
                foreach (Entity ingredient in response.Entities)
                {
                    logger.debug("Ingredient loop: " + ingredient);

                    // this section of the loop will build the recipe text for the dc_recipeingredients field
                    amount = 0.0;
                    portionType = null;
                    ingredientName = null;

                    amount = Convert.ToDouble(ingredient.Attributes["dc_portionsize"]);
                    logger.debug("Amount = " + amount);

                    portionType = ((EntityReference)ingredient["dc_portiontypeid"]).Name;
                    logger.debug("portionType = " + portionType);
                    ingredientName = (String)ingredient["dc_name"];
                    logger.debug("ingredientName = " + ingredientName);
                    logger.debug("Recipe building: adding " + amount + " " + portionType + " " + ingredientName);
                    buildRecipe += (amount + " " + portionType + " of " + ingredientName + "<br><br>");


                    Guid foodId = !ingredient.Contains("dc_foodingredientid") ? Guid.Empty : ((EntityReference)ingredient["dc_foodingredientid"]).Id;
                    logger.debug("foodId: " + foodId);
                    #region fetch xml for the food
                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
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

                    fetchXml = fetchXml.Replace("@ID", foodId.ToString());
                    EntityCollection foodResponse = null;
                    try
                    {
                        foodResponse = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                    }
                    catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                    {
                        logger.error("Code: " + ex.Detail.ErrorCode);
                        logger.error("Message: " + ex.Detail.Message);
                        logger.error("Trace: " + ex.Detail.TraceText);
                        logger.error("Inner Fault: " + ex.Detail.InnerFault);
                    }
                    logger.debug("Records found: " + foodResponse.Entities.Count);

                    double portionAdjust = findPortionMultiplier(foodResponse.Entities[0], ingredient);
                    logger.debug("recieved portionAdjust amount " + portionAdjust);

                    // accumulating the gram weight of ingredients requires the poriton adjustment value
                    ingredientGrams += (Convert.ToDouble(ingredient.Attributes["dc_originalgramweight"]) * portionAdjust);
                    logger.debug("Ingredient weight in grams = " + ingredientGrams);

                    #region  calculating the nutrient values
                    if (foodResponse != null && foodResponse.Entities.Count() > 0)
                    {
                        Entity foodNutrient = foodResponse.Entities[0];

                        logger.debug("Retrieving Kcals");
                        if (foodNutrient.Contains("aa.dc_kcals"))
                        {
                            logger.debug("Type of dc_kcals " + ((AliasedValue)foodNutrient["aa.dc_kcals"]).Value.GetType());
                            double foodKcals = foodNutrient.Contains("aa.dc_kcals") ? (double)((AliasedValue)foodNutrient["aa.dc_kcals"]).Value : 0;
                            logger.debug("Kcals: " + foodKcals + " food/ingredient portions: " + (foodKcals * portionAdjust));
                            kcalsSum += Convert.ToDecimal(foodKcals * portionAdjust);
                            logger.debug("kcals Sum: " + kcalsSum);
                        }
                        else
                        {
                            logger.debug("Kcals not found (aa.dc_kcals)");
                        }
                        if (foodNutrient.Contains("dc_unit_gram_weight"))
                        {
                            logger.debug("Type of dc_unit_gram_weight " + (foodNutrient["dc_unit_gram_weight"]).GetType());
                            double unitGramWeightTemp = foodNutrient.Contains("dc_unit_gram_weight") ? (double)(foodNutrient["dc_unit_gram_weight"]) : 0;
                            logger.debug("Unit Gram Weight: " + unitGramWeightTemp + " food/ingredient portions: " + (unitGramWeightTemp * portionAdjust));
                            unitGramWeightSum += unitGramWeightTemp * portionAdjust;
                            logger.debug("Unit Gram Weight Sum: " + unitGramWeightTemp);
                        }
                        double portionAmount = foodNutrient.Contains("dc_portion_amount") ? 0 : (double)foodNutrient["dc_portion_amount"];
                        logger.debug("portionAmount: " + portionAmount);
                        if (foodNutrient.Contains("dc_portion_amount"))
                        {
                            logger.debug("Type of dc_portion_amount " + foodNutrient["dc_portion_amount"].GetType());
                        }

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

                        if (foodNutrient.Contains("aa.dc_glycemic_load_a"))
                        {
                            double value = foodNutrient.Contains("aa.dc_glycemic_load_a") ? (double)((AliasedValue)foodNutrient["aa.dc_glycemic_load_a"]).Value : 0;
                            dc_glycemic_load_aSum += value * portionAdjust;
                        }
                        else
                        {
                            logger.debug("there is no Glycemic load");
                        }

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


                    }
                    #endregion

                }// end of the foreach through ingredients
                //logger.debug("unitGramWeight: " + unitGramWeight);
                logger.debug("unitGramWeightSum: " + unitGramWeightSum);

                unitGramWeightMultipler = numberOfServings;
                //double numberOfServings = unitGramWeightSum / unitGramWeight;
                double unitGramWeight = unitGramWeightSum / (numberOfServings);
                logger.debug("unitGramWeightMultipler: " + unitGramWeightMultipler);
                logger.debug("numberOfServings: " + numberOfServings);

                logger.debug("KcalSum: " + kcalsSum);

                logger.debug("Recipe Builder: ingredient loop ended \r\nbuildRecipe : \r\n" + buildRecipe);

                #region Building the nutrients in an entity
                //build nutrient information
                Entity nutrient = new Entity("dc_food_nutrients");
                logger.debug("Entity to build dc_food_nutrients created");
                nutrient["dc_fat"] = fatSum / unitGramWeightMultipler;
                logger.debug("dc_fat added to dc_food_nutrients");
                nutrient["dc_carbohydrate"] = carbohydrateSum / unitGramWeightMultipler;
                logger.debug("dc_carbohydrate added to dc_food_nutrients");
                nutrient["dc_protein"] = proteinSum / unitGramWeightMultipler;
                logger.debug("dc_protein added to dc_food_nutrients");
                nutrient["dc_alcohol"] = dc_alcoholSum / unitGramWeightMultipler;
                nutrient["dc_sugar"] = dc_sugarSum / unitGramWeightMultipler;
                nutrient["dc_fa_trans"] = dc_fa_transSum / unitGramWeightMultipler;
                nutrient["dc_cholestrol"] = dc_cholestrolSum / unitGramWeightMultipler;
                nutrient["dc_fiber"] = dc_fiberSum / unitGramWeightMultipler;
                nutrient["dc_fa_sat"] = dc_fa_satSum / unitGramWeightMultipler;
                nutrient["dc_fa_unsat"] = dc_fa_unsatSum / unitGramWeightMultipler;
                nutrient["dc_fa_mono"] = dc_fa_monoSum / unitGramWeightMultipler;
                nutrient["dc_fa_poly"] = dc_fa_polySum / unitGramWeightMultipler;
                nutrient["dc_vit_c"] = dc_vit_cSum / unitGramWeightMultipler;
                nutrient["dc_vit_a"] = dc_vit_aSum / unitGramWeightMultipler;
                nutrient["dc_vit_a_iu"] = dc_vit_a_iuSum / unitGramWeightMultipler;
                nutrient["dc_vit_d"] = dc_vit_dSum / unitGramWeightMultipler;
                nutrient["dc_vitamindd2d3ing"] = dc_vitamindd2d3ingSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_vit_e"] = dc_vit_eSum / unitGramWeightMultipler;
                nutrient["dc_vit_k"] = dc_vit_kSum / unitGramWeightMultipler;
                nutrient["dc_thiamin"] = dc_thiaminSum / unitGramWeightMultipler;
                nutrient["dc_riboflavin"] = dc_riboflavinSum / unitGramWeightMultipler;
                nutrient["dc_folate_dfe"] = dc_folate_dfeSum / unitGramWeightMultipler;
                nutrient["dc_folate"] = dc_folateSum / unitGramWeightMultipler;
                nutrient["dc_niacin"] = dc_niacinSum / unitGramWeightMultipler;
                nutrient["dc_vit_b12"] = dc_vit_b12Sum / unitGramWeightMultipler;
                nutrient["dc_panto_acid"] = dc_panto_acidSum / unitGramWeightMultipler;
                nutrient["dc_vit_b6"] = dc_vit_b6Sum / unitGramWeightMultipler;
                nutrient["dc_choline"] = dc_cholineSum / unitGramWeightMultipler;
                nutrient["dc_biotin"] = dc_biotinSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_chromium"] = dc_chromiumSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_calcium"] = dc_calciumSum / unitGramWeightMultipler;
                nutrient["dc_potassium"] = dc_potassiumSum / unitGramWeightMultipler;
                nutrient["dc_magnesium"] = dc_magnesiumSum / unitGramWeightMultipler;
                nutrient["dc_sodium"] = dc_sodiumSum / unitGramWeightMultipler;
                nutrient["dc_phosphorus"] = dc_phosphorusSum / unitGramWeightMultipler;
                nutrient["dc_manganese"] = dc_manganeseSum / unitGramWeightMultipler;
                nutrient["dc_zinc"] = dc_zincSum / unitGramWeightMultipler;
                nutrient["dc_iron"] = dc_ironSum / unitGramWeightMultipler;
                nutrient["dc_copper"] = dc_copperSum / unitGramWeightMultipler;
                nutrient["dc_selenium"] = dc_seleniumSum / unitGramWeightMultipler;
                nutrient["dc_starch"] = dc_starchSum / unitGramWeightMultipler;
                nutrient["dc_sucrose"] = dc_sucroseSum / unitGramWeightMultipler;
                nutrient["dc_glucose"] = dc_glucoseSum / unitGramWeightMultipler;
                nutrient["dc_fructose"] = dc_fructoseSum / unitGramWeightMultipler;
                nutrient["dc_maltose"] = dc_maltoseSum / unitGramWeightMultipler;
                nutrient["dc_galactose"] = dc_galactoseSum / unitGramWeightMultipler;
                nutrient["dc_lactose"] = dc_lactoseSum / unitGramWeightMultipler;
                nutrient["dc_alanine"] = dc_alanineSum / unitGramWeightMultipler;
                nutrient["dc_arginine"] = dc_arginineSum / unitGramWeightMultipler;
                nutrient["dc_aspartic_acid"] = dc_aspartic_acidSum / unitGramWeightMultipler;
                nutrient["dc_cystine"] = dc_cystineSum / unitGramWeightMultipler;
                nutrient["dc_glutamic_acid"] = dc_glutamic_acidSum / unitGramWeightMultipler;
                nutrient["dc_glycine"] = dc_glycineSum / unitGramWeightMultipler;
                nutrient["dc_histidine"] = dc_histidineSum / unitGramWeightMultipler;
                nutrient["dc_isoleucine"] = dc_isoleucineSum / unitGramWeightMultipler;
                nutrient["dc_leucine"] = dc_leucineSum / unitGramWeightMultipler;
                nutrient["dc_lysine"] = dc_lysineSum / unitGramWeightMultipler;
                nutrient["dc_methionine"] = dc_methionineSum / unitGramWeightMultipler;
                nutrient["dc_phenylalanine"] = dc_phenylalanineSum / unitGramWeightMultipler;
                nutrient["dc_proline"] = dc_prolineSum / unitGramWeightMultipler;
                nutrient["dc_serine"] = dc_serineSum / unitGramWeightMultipler;
                nutrient["dc_threonine"] = dc_threonineSum / unitGramWeightMultipler;
                nutrient["dc_tryptophan"] = dc_tryptophanSum / unitGramWeightMultipler;
                nutrient["dc_tyrosine"] = dc_tyrosineSum / unitGramWeightMultipler;
                nutrient["dc_valine"] = dc_valineSum / unitGramWeightMultipler;
                //nutrient["dc_sfa40butyricacidg"] = dc_sfa40butyricacidgSum.ToString();        //single line of text not a double or decimal
                //nutrient["dc_sfa60caproicacidg"] = dc_sfa60caproicacidgSum / unitGramWeightMultipler;                   //single line of text not a double or decimal
                nutrient["dc_lipids_8_0"] = dc_lipids_8_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_10_0"] = dc_lipids_10_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_12_0"] = dc_lipids_12_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_14_0"] = dc_lipids_14_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_16_0"] = dc_lipids_16_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_18_0"] = dc_lipids_18_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_20_0"] = dc_lipids_20_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_22_0"] = dc_lipids_22_0Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_14_1"] = dc_lipids_14_1Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_16_1"] = dc_lipids_16_1Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_18_1"] = dc_lipids_18_1Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_20_1"] = dc_lipids_20_1Sum / unitGramWeightMultipler;
                nutrient["dc_mufa221erucicacidg"] = dc_mufa221erucicacidgSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_lipids_18_2"] = dc_lipids_18_2Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_18_3"] = dc_lipids_18_3Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_18_4"] = dc_lipids_18_4Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_20_4"] = dc_lipids_20_4Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_20_5_n3"] = dc_lipids_20_5_n3Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_22_5_n3"] = dc_lipids_22_5_n3Sum / unitGramWeightMultipler;
                nutrient["dc_lipids_22_6_n3"] = dc_lipids_22_6_n3Sum / unitGramWeightMultipler;
                nutrient["dc_phytosterolsmg"] = dc_phytosterolsmgSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_stigmasterol"] = dc_stigmasterolSum / unitGramWeightMultipler;
                nutrient["dc_campesterol"] = dc_campesterolSum / unitGramWeightMultipler;
                nutrient["dc_betas_sitosterol"] = dc_betas_sitosterolSum / unitGramWeightMultipler;
                nutrient["dc_glycemic_index_a"] = dc_glycemic_index_aSum / unitGramWeightMultipler;
                //nutrient["dc_glycemicindexlmh"] = dc_glycemicindexlmhSum / unitGramWeightMultipler;         //optionset
                nutrient["dc_glycemic_load_a"] = dc_glycemic_load_aSum / unitGramWeightMultipler;
                //nutrient["dc_glycemicloadlmh"] = dc_glycemicloadlmhSum / unitGramWeightMultipler;             //optionset
                nutrient["dc_folateaddedg"] = dc_folateaddedgSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_folateinfoodg"] = dc_folateinfoodgSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_retinol"] = dc_retinolSum / unitGramWeightMultipler;
                nutrient["dc_carotene_alpha"] = dc_carotene_alphaSum / unitGramWeightMultipler;
                nutrient["dc_carotene_beta"] = dc_carotene_betaSum / unitGramWeightMultipler;
                nutrient["dc_lycopene"] = dc_lycopeneSum / unitGramWeightMultipler;
                nutrient["dc_lutein_zeaxanthin"] = dc_lutein_zeaxanthinSum / unitGramWeightMultipler;
                nutrient["dc_cryptoxanthin_beta"] = dc_cryptoxanthin_betaSum / unitGramWeightMultipler;
                nutrient["dc_vitkdihydrophylloquinoneg"] = dc_vitkdihydrophylloquinonegSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_vitkmenaquinone4ing"] = dc_vitkmenaquinone4ingSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_tocopherol_beta"] = dc_tocopherol_betaSum / unitGramWeightMultipler;
                nutrient["dc_tocopherol_delta"] = dc_tocopherol_deltaSum / unitGramWeightMultipler;
                nutrient["dc_tocopherol_gamma"] = dc_tocopherol_gammaSum / unitGramWeightMultipler;
                nutrient["dc_vitamind2ergoing"] = dc_vitamind2ergoingSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_vitamind3choleing"] = dc_vitamind3choleingSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_betaine"] = dc_betaineSum / unitGramWeightMultipler;
                nutrient["dc_theobrominemg"] = dc_theobrominemgSum / (decimal)unitGramWeightMultipler;
                nutrient["dc_caffeinemg"] = dc_caffeinemgSum / (decimal)unitGramWeightMultipler;

                nutrient["dc_kcals"] = Convert.ToDouble(kcalsSum / (decimal)unitGramWeightMultipler);
                #endregion


                //verify if the parent food has a dc_food_nutrient. If not create the dc_food_nutrient with the relationship
                Entity nutrientResponse = null;
                try
                {
                    nutrientResponse = crmService.Retrieve("dc_foods", parentFoodId, new ColumnSet(new String[] { "dc_name", "dc_foodnutrientid", "dc_recipeingredients", "dc_unit_gram_weight" }));
                    logger.debug("Parent food entity found, adding built recipe");
                    //double servings = ingredientGrams / (Convert.ToDouble(nutrientResponse.Attributes["dc_unit_gram_weight"]));
                    nutrientResponse["dc_recipeingredients"] = buildRecipe;
                    //nutrientResponse["dc_numberofservings"]     = ingredientGrams / (Convert.ToDouble(nutrientResponse.Attributes["dc_unit_gram_weight"]));
                    crmService.Update(nutrientResponse);

                }
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                {
                    logger.error("Code: " + ex.Detail.ErrorCode);
                    logger.error("Message: " + ex.Detail.Message);
                    logger.error("Trace: " + ex.Detail.TraceText);
                    logger.error("Inner Fault: " + ex.Detail.InnerFault);
                }

                if (nutrientResponse.Contains("dc_foodnutrientid"))
                {
                    logger.debug("foodnutrient exists. Updating.");

                    //update the values in CRM
                    try
                    {
                        Guid nutrientId = ((EntityReference)nutrientResponse["dc_foodnutrientid"]).Id;
                        logger.debug("nutrientId Guid created");
                        nutrient["dc_food_nutrientsid"] = nutrientId;
                        logger.debug("nutrient dc_food_nutrientsid assigned");
                        logger.debug("nutrient information updated");
                        crmService.Update(nutrient);

                        if (updateParent)
                        {
                            Entity parentFood = new Entity("dc_foods");
                            parentFood["dc_foodsid"] = parentFoodId;
                            //parentFood["dc_numberofservings"] = numberOfServings;
                            parentFood["dc_unit_gram_weight"] = unitGramWeight;
                            parentFood["dc_recipegramweight"] = Convert.ToDecimal(unitGramWeightSum);
                            crmService.Update(parentFood);

                            logger.debug("Nutrient updated");
                            return (unitGramWeight);
                        }

                    }
                    catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                    {
                        logger.error("nutrient update exception catch is firing");
                        logger.error("Code: " + ex.Detail.ErrorCode);
                        logger.error("Message: " + ex.Detail.Message);
                        logger.error("Ex.mess: " + ex.Message);
                        logger.error("Ex.stacktrace: " + ex.StackTrace);
                        logger.error("Trace: " + ex.Detail.TraceText);
                        logger.error("Inner Fault: " + ex.Detail.InnerFault);
                    }

                }
                else
                {
                    logger.debug("No foodnutrient found " + nutrientResponse.ToString());

                    // create  a new dc_food nutrient
                    nutrient["dc_name"] = nutrientResponse["dc_name"];
                    logger.debug("dc_name added to the nutrient dc_name field");

                    //create entity in CRM and add the reference to the parent food
                    Guid nutri = crmService.Create(nutrient);
                    logger.debug("Guid created");
                    EntityReference n = new EntityReference("dc_foodnutrientid", nutri);
                    logger.debug("EntityReference Created");
                    nutrientResponse["dc_foodnutrientid"] = n;

                    if (updateParent)
                    {
                        Entity parentFood = new Entity("dc_foods");
                        parentFood["dc_foodsid"] = parentFoodId;
                        parentFood["dc_foodnutrientid"] = new EntityReference("dc_food_nutrients", nutri);
                        //parentFood["dc_numberofservings"] = numberOfServings;
                        parentFood["dc_unit_gram_weight"] = unitGramWeight;
                        parentFood["dc_recipegramweight"] = Convert.ToDecimal(unitGramWeightSum);
                        nutrientResponse["dc_recipeingredients"] = buildRecipe;
                        crmService.Update(parentFood);

                        logger.debug("new dc_food_nutrients added to the parentFood");
                        return (unitGramWeight);
                    }
                }

            }
            return (-1);
        }
        // finds the conversion multiplier between the ingredient and the food entity the ingredient is based on (not used in)
        private double findPortionMultiplier(Entity food, Entity ingredient)
        {
            logger.debug("Entered multiplier method");

            //Guid foodPortionId = ((EntityReference)food["dc_portiontypeid"]).Id;
            //Guid ingredientPortionID = ((EntityReference)ingredient["dc_portiontypeid"]).Id;

            


            //handles the matching portion type
            if ( food.Contains("dc_portiontypeid") && ingredient.Contains("dc_portiontypeid") &&
                ((EntityReference)food["dc_portiontypeid"]).Id == ((EntityReference)ingredient["dc_portiontypeid"]).Id )
            {
                logger.debug("Check Success: food and ingredient have the same dc_portiontypeid so should enter the if statement");
                double num1 = Convert.ToDouble(ingredient.Attributes["dc_portionsize"]);
                double num2 = Convert.ToDouble(food.Attributes["dc_portion_amount"]);

                logger.debug("num1: " + num1);
                logger.debug("num2 :" + num2);
                return num1 / num2;
            }
            else
            {
                //handles different portion type needs to convert

                // call helper file that will find the conversion factor based on the names returned by the entity reference above
                //(foodPortionId ingredientPortionID) convert the food to the same portion type as ingredient then find multiplier
                if (food.Contains("dc_portiontypeid") && ingredient.Contains("dc_portiontypeid"))
                {
                    logger.debug("Calling conversion");
                    double convert = new ConversionFactor(food, ingredient).getConversionFactor();
                    logger.debug("Convertion Factor from helper = " + convert);

                    double ingredientPortion = Convert.ToDouble(ingredient.Attributes["dc_portionsize"]);
                    double foodPortion = Convert.ToDouble(food.Attributes["dc_portion_amount"]);
                    logger.debug("foodPortion original = " + foodPortion + " ingredient original = " + ingredientPortion);

                    foodPortion *= convert;

                    logger.debug("foodPortion after conversion factor = " + foodPortion);

                    double multiplier = ingredientPortion / foodPortion;
                    logger.debug("returning portion multiplier of = " + multiplier);
                    return multiplier;
                }
                else
                {
                    logger.debug("Food or ingredient is missing portiontypeid");
                    if (food.Contains("dc_portiontypeid"))
                    {
                        logger.debug("FoodId: "+ ((EntityReference)food["dc_portiontypeid"]).Id);
                    }
                    if (ingredient.Contains("dc_portiontypeid"))
                    {
                        logger.debug("ingredient Id: " + ((EntityReference)ingredient["dc_portiontypeid"]).Id);
                    }
                }
            }

            /*logger.debug("Check Failure: food and ingredient are not appearing equal so will skip the math");*/
            logger.debug("Returning Default Multiplier");
            return 1.0;
        }
    }
}
