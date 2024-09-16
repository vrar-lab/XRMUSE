using TMPro;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    public class ScreenObjectives : MonoBehaviour
    {
        public Sprite[] materialsSprites;
        public Sprite[] toolsSprites;
        public int[] recipesMat1, recipesMat2, recipesTool, recipesResult;
        public CombinationPanelController combinationPanel;
        public ObjectivePanelController objectivePanel;
        public TMP_Text textTitleRecipes;
        public TMP_Text textTitleObjectives;

        void Start()
        {
            if (materialsSprites == null || materialsSprites.Length == 0)
                materialsSprites = SD_EventMain_ScreenObjectivesIconLoad.materialsSprites;
            if (toolsSprites == null || toolsSprites.Length == 0)
                toolsSprites = SD_EventMain_ScreenObjectivesIconLoad.toolsSprites;
        }
    }
}