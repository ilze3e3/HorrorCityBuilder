using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class BuildingSelectFunction : MonoBehaviour
{
    Camera mainCamera;
    public bool isSelectFunction = true;

    public GameObject descriptionPanel;
    public TextMeshProUGUI tileNameText;
    public TextMeshProUGUI tileEffectText;
    public Button demolishButton;
    public Button exitButton;

    public WorldSpawner worldSpawner;

    public UnityEvent TurnOffSelectFunctionEvent;
    public UnityEvent TurnOnSelectFunctionEvent;

    private void Awake()
    {
        mainCamera = this.gameObject.GetComponent<Camera>();
        TurnOffSelectFunctionEvent = new UnityEvent();
        TurnOnSelectFunctionEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(delegate { TurnOffPanel(); });

        TurnOffSelectFunctionEvent.AddListener(TurnOffPanel);
        TurnOffSelectFunctionEvent.AddListener(TurnOffSelectFunction);

        TurnOnSelectFunctionEvent.AddListener(TurnOnSelectFunction);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelectFunction)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    demolishButton.onClick.RemoveAllListeners();
                    GameObject hitObject = hitInfo.collider.gameObject;
                    if (hitObject != null)
                    {
                        Tile hitTile = hitObject.GetComponent<TileAttached>().tileAttached;
                        descriptionPanel.SetActive(true);
                        tileNameText.text = hitTile.tileName;
                        string tmp = "Effects: \n";
                        foreach (EffectsToResources e in hitTile.tileEffects)
                        {
                            string sign = "";
                            if (e.affectedQuantity > 0) sign = "+";
                            tmp += "- " + e.affectedResourceType + " " + sign + e.affectedQuantity + "\n";
                        }
                        tileEffectText.text = tmp;

                        if (hitTile.tileType != ResourceSystem.ResourceAndBuildingRecords.Grass)
                        {
                            demolishButton.gameObject.SetActive(true);
                            demolishButton.onClick.AddListener(delegate { worldSpawner.DemolishBuilding(hitObject); });
                            demolishButton.onClick.AddListener(delegate { TurnOffPanel(); });
                        }
                        else
                        {
                            demolishButton.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void TurnOffPanel()
    {
        descriptionPanel.SetActive(false);
        demolishButton.onClick.RemoveAllListeners();
    }
    private void TurnOffSelectFunction() => isSelectFunction = false;
    private void TurnOnSelectFunction() => isSelectFunction = true;
}
