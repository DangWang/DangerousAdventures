using UnityEngine;

namespace Mirror
{
    public class script_Door : MonoBehaviour
    {
        public bool closed = true;
        public Sprite closedSprite;
        public Sprite openSprite;

        public void ToggleDoor()
        {
            if (closed)
            {
                GetComponent<SpriteRenderer>().sprite = openSprite;
                GetComponent<script_Tile>().occupied = false;
                gameObject.tag = "OpenDoor";
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = closedSprite;
                GetComponent<script_Tile>().occupied = true;
                gameObject.tag = "Door";
            }

            GetComponent<AudioSource>().Play();
            closed = !closed;
        }
    }
}