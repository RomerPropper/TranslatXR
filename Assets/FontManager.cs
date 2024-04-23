using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;



public class FontManager : MonoBehaviour
{
    private TextMeshProUGUI tmp_font;
    public int fontSize;
    private plyerManagement playerManager;
    public GameObject objectPrefab; // Prefab for representing other players' right hands
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
       // GetComponent<Button>().onClick.AddListener(OnButtonClick);
        tmp_font.fontSize = fontSize;
    }

    public void FontUp()
    {
       if (playerManager == null) return;
        tmp_font.fontSize += 5;
        Dictionary<int, Vector3> rightHandPositions = playerManager.GetAllheadPositions();
        int localPlayerID = playerManager.localAvatar != null ? playerManager.localAvatar.realtimeView.ownerIDSelf : -1;

        // Update or create GameObjects for each player's right hand
        foreach (KeyValuePair<int, Vector3> entry in rightHandPositions)
        {
            // Update the position of the existing GameObject
            tmp_font = playerObjects[entry.Key].GetComponent< TextMeshProUGUI > ();
            tmp_font.fontSize += 5;
        }
    }
    public void FontDown()
    {
        tmp_font.fontSize -= 5;
    }
}
