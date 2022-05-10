using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualMagnetizeInput()
        {
            starterAssetsInputs.MagnetizeInput();
        }

        public void VirtualPauseInput(bool virtualPauseState)
        {
            starterAssetsInputs.PauseInput(virtualPauseState);
        }

    }

}
