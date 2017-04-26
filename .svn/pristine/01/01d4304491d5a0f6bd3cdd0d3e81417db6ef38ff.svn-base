using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers
{
    class Fitness
    {
        public enum IncrementalActivityLevel { Sedentary = 1, LightlyActive = 2, ModeratelyToHighlyActive = 3 }

        /// <summary>
        /// Calculate the incremental PA value
        /// </summary>
        /// <param name="gendeCodeValue"></param>
        /// <param name="ageValue"></param>
        /// <param name="overweight"></param>
        /// <returns></returns>
        public static decimal RetrieveLowValuePA(int gendeCodeValue, int ageValue, bool overweight, IncrementalActivityLevel activityLevel)
        {
            //Male
            if (gendeCodeValue == 1)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.14m);
                    }

                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.13m);
                    }

                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.11m);
                    }

                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.13m);
                    }
                }
            }
            //Female
            else if (gendeCodeValue == 2)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.17m);
                    }
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.18m);
                    }

                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.12m);
                    }

                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (1.01m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (1.15m);
                    }
                }
            }
            return (1m);
        }
        /// <summary>
        /// Calculate the incremental PA value
        /// </summary>
        /// <param name="gendeCodeValue"></param>
        /// <param name="ageValue"></param>
        /// <param name="overweight"></param>
        /// <returns></returns>
        public static decimal CalculateIncrementalPA(int gendeCodeValue, int ageValue, bool overweight, IncrementalActivityLevel activityLevel)
        {
            //Male
            if (gendeCodeValue == 1)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.12m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.28m);
                    }

                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.11m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.32m);
                    }

                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.37m);
                    }

                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.11m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.41m);
                    }
                }
            }
            //Female
            else if (gendeCodeValue == 2)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.15m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.39m);
                    }
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.17m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.42m);
                    }

                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.11m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.33m);
                    }

                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == IncrementalActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.LightlyActive)
                    {
                        return (.13m);
                    }
                    else if (activityLevel == IncrementalActivityLevel.ModeratelyToHighlyActive)
                    {
                        return (.30m);
                    }
                }
            }
            return (1m);
        }
        /// <summary>
        /// Calculate BMI
        /// </summary>
        /// <param name="weightKgValue"></param>
        /// <param name="heightInMeters"></param>
        /// <returns></returns>
        public static decimal CalculateBMI(decimal weightKgValue, decimal heightInMeters)
        {
            return (weightKgValue / (heightInMeters * heightInMeters));
        }
        /// <summary>
        /// Figure out if person is overweight
        /// </summary>
        /// <param name="BMI"></param>
        /// <returns></returns>
        public static bool IsOverWeight(decimal BMI)
        {
            if (BMI > 25)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static decimal CalculateKcals(int genderCode, int ageValue, decimal PA, decimal weightKgValue, decimal heightMValue, bool overweight)
        {
            //males
            if (genderCode == 1)
            {
                if (ageValue >= 3 && ageValue <= 8 && !overweight)
                {
                    return (88.5m - (61.9m * ageValue) + PA * ((26.7m * weightKgValue) + (903m * heightMValue)) + 20m);
                }
                else if (ageValue >= 9 && ageValue <= 18 && !overweight)
                {
                    return (88.5m - (61.9m * ageValue) + PA * ((26.7m * weightKgValue) + (903m * heightMValue)) + 25m);
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    return (114m - (50.9m * ageValue) + PA * ((19.5m * weightKgValue) + (1161.4m * heightMValue)));
                }
                else if (ageValue >= 19 && !overweight)
                {
                    return (662m - (9.53m * ageValue) + PA * ((15.91m * weightKgValue) + (539.6m * heightMValue)));
                }
                else if (ageValue >= 19 && overweight)
                {
                    return (864m - (9.72m * ageValue) + PA * ((14.2m * weightKgValue) + (503m * heightMValue)));
                }
            }
            //females
            else if (genderCode == 2)
            {
                if (ageValue >= 3 && ageValue <= 8 && !overweight)
                {
                    return (135.3m - (30.8m * ageValue) + PA * ((10.0m * weightKgValue) + (934m * heightMValue)) + 20m);
                }
                else if (ageValue >= 9 && ageValue <= 18 && !overweight)
                {
                    return (135.3m - (30.8m * ageValue) + PA * ((10.0m * weightKgValue) + (934m * heightMValue)) + 25m);
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    return (389m - (41.2m * ageValue) + PA * ((15.0m * weightKgValue) + (701.6m * heightMValue)));
                }
                else if (ageValue >= 19 && !overweight)
                {
                    return (354m - (6.91m * ageValue) + PA * ((9.36m * weightKgValue) + (726m * heightMValue)));
                }
                else if (ageValue >= 19 && overweight)
                {
                    return (387m - (7.31m * ageValue) + PA * ((10.9m * weightKgValue) + (660.7m * heightMValue)));
                }
            }
            return (1);
        }
    }
}
