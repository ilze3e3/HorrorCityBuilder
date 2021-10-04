using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementScript : MonoBehaviour
{
    public bool isPlacingStatus = false;
    public GameObject modelToPlace;
    public Tile tileToPlace;
    Camera mainCamera; 
    RaycastHit hitInfo = new RaycastHit();
    GameObject hologram;
    public Vector3 hologramInitialPosition;

    public ResourceSystem resourceSystem;

    public UnityEvent resetBuildingMethods;
    public UnityEvent buildingInProgress;
    public UnityEvent buildingPlaced;

    public BuildingSelectFunction selectFunction;
    public DayNightCycle dayNightCycle;

    IEnumerator DelayToStartSelectFunction()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            buildingPlaced.Invoke();
            break;
        }
    }

    private void Awake()
    {
        mainCamera = this.gameObject.GetComponent<Camera>();
        resetBuildingMethods = new UnityEvent();
        buildingInProgress = new UnityEvent();
    }
    // Start is called before the first frame update
    void Start()
    {
        buildingInProgress.AddListener(delegate { selectFunction.TurnOffSelectFunctionEvent.Invoke(); });
        buildingInProgress.AddListener(delegate { dayNightCycle.PauseGame(); });

        buildingPlaced.AddListener(delegate { selectFunction.TurnOnSelectFunctionEvent.Invoke(); });
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacingStatus)
        {

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                Debug.Log("Object Hit is " + hitInfo.collider.gameObject.name);
                hologram.transform.position = hitInfo.collider.transform.position;
                // Get the centre location of the collider.gameobject
                // Set the location of the hologram to that location. 

                if (hitInfo.collider.gameObject.layer != 6)
                {
                    Debug.Log("Building CAN be placed");
                    // Make the hologram green to show that it can be placed
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Place building down and deduct money. 
                        GameObject oldTileObject = hitInfo.collider.gameObject;
                        Vector3 position = oldTileObject.transform.position;
                        Tile oldTile = oldTileObject.GetComponent<TileAttached>().tileAttached;
                        resourceSystem.RemoveTileRecord(oldTile, oldTileObject.transform);
                        Destroy(hologram);
                        Destroy(oldTileObject);
                        GameObject newBuilding = Instantiate(modelToPlace, position, new Quaternion());
                        resourceSystem.InsertTileRecord(tileToPlace, newBuilding.transform);
                        StartCoroutine("DelayToStartSelectFunction");
                        resetBuildingMethods.Invoke();
                        resetBuildingMethods.RemoveAllListeners();
                    }
                }
                else
                {
                    // Make the hologram red to show that it cannot be placed
                    Debug.Log("Building CANNOT be placed");
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                //isPlacingStatus = false;
                resetBuildingMethods.RemoveAllListeners();
            }
        }
    }

    public void PlaceBuilding(StoreSlots slot)
    {
        buildingInProgress.Invoke();

        resetBuildingMethods.AddListener(delegate { dayNightCycle.UnPauseGame(); });
        resetBuildingMethods.AddListener(delegate { resourceSystem.ApplyResourceEffects(slot.tile.tileEffects); });
        resetBuildingMethods.AddListener(delegate { resourceSystem.DeductMoney(slot.price); });
        resetBuildingMethods.AddListener(delegate { TurnOffBuildingMode(); });
        
        modelToPlace = slot.model;
        tileToPlace = slot.tile;
        isPlacingStatus = true;
        Debug.Log(isPlacingStatus);
        hologram = Instantiate(modelToPlace, hologramInitialPosition, new Quaternion());
        Destroy(hologram.GetComponent<Collider>());
    }
    public void TurnOffBuildingMode() => isPlacingStatus = false;
}
