using UnityEngine;

namespace Player
{
    public class PlayerAnim
    {
        private const float speed = 0.5f;
        private float blend;

        public float AnimUpdate(float mouseX, bool isLMB)
        {
            if (isLMB)
            {
                blend = Mathf.Lerp(blend, mouseX, speed * Time.deltaTime);
                blend = Mathf.Clamp(blend, -1, 1);
            }
            else
            {
                blend = Mathf.Lerp(blend, 0, 5 * Time.deltaTime);
            }

            return blend;
        }
    }
}
