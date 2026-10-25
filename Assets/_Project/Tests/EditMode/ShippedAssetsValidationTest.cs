using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.Tests
{
    /// <summary>
    /// Regression guard: every WheelConfigSO asset in the project must pass Core validation.
    /// </summary>
    [TestFixture]
    public sealed class ShippedAssetsValidationTest
    {
        [Test]
        public void AllWheelConfigs_PassValidation()
        {
            string[] guids = AssetDatabase.FindAssets("t:WheelConfigSO");

            if (guids.Length == 0)
            {
                Assert.Inconclusive("No WheelConfigSO assets found yet — create them in the Editor.");
                return;
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var wheel = AssetDatabase.LoadAssetAtPath<WheelConfigSO>(path);
                Assert.IsNotNull(wheel, $"Failed to load {path}");

                var errors = WheelDefinitionValidator.Validate(wheel);
                Assert.AreEqual(0, errors.Count,
                    $"Validation errors in '{wheel.name}' ({path}):\n{string.Join("\n", errors)}");
            }
        }
    }
}
