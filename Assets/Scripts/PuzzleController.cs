using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleController : MonoBehaviour
{
    // AR camera
    [Header("Cameras")]
    [SerializeField] private Camera ARCamera;

    // models
    [Header("Models")]
    [SerializeField] private GameObject Model;
    [SerializeField] private GameObject WholeModels;

    // particle system/confetti
    [Header("Confetti")]
    [SerializeField] private ParticleSystem Confetti;

    // UI text field
    [Header("Texts")]
    [SerializeField] private Text TimeLapsedText;
    [SerializeField] private Text DistanceText;
    [SerializeField] private Text AnchorDistText;
    [SerializeField] private Text RotationText;
    [SerializeField] private Text AngleText;
    [SerializeField] private Text DeltaText;
    [SerializeField] private Text ResultTimeText;
    [SerializeField] private Text TargetsFoundText;
    [SerializeField] private Text MarkersListText;

    // solution markers
    [Header("Solution Markers")]
    [SerializeField] private GameObject SolutionMarker1;
    [SerializeField] private GameObject SolutionMarker2;
    [SerializeField] private GameObject SolutionMarker3;
    [SerializeField] private GameObject SolutionMarker4;
    [SerializeField] private GameObject SolutionMarker5;

    // distance
    [Header("Distance")]
    [SerializeField] private float CorrectDistance1;
    [SerializeField] private float CorrectDistance2;
    [SerializeField] private float CorrectDistance3;
    [SerializeField] private float CorrectDistance4;
    [SerializeField] private float CorrectDistance5;
    [SerializeField] private float ErrorDistance;

    // anchor distance
    [Header("Anchor Distance")]
    [SerializeField] private float CorrectAnchorDist1;
    [SerializeField] private float CorrectAnchorDist2;
    [SerializeField] private float CorrectAnchorDist3;
    [SerializeField] private float CorrectAnchorDist4;
    [SerializeField] private float CorrectAnchorDist5;
    [SerializeField] private float ErrorAnchorDist;

    // rotations
    [Header("Rotations")]
    [SerializeField] private float CorrectRotation1;
    [SerializeField] private float CorrectRotation2;
    [SerializeField] private float CorrectRotation3;
    [SerializeField] private float CorrectRotation4;
    [SerializeField] private float CorrectRotation5;
    [SerializeField] private float ErrorRotation;


    // angles
    [Header("Delta")]
    [SerializeField] private float DeltaValue1;
    [SerializeField] private float DeltaValue2;
    [SerializeField] private float DeltaValue3;
    [SerializeField] private float DeltaValue4;
    [SerializeField] private float DeltaValue5;
    [SerializeField] private float DeltaError;

    // panels
    [Header("Panels")]
    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private GameObject PreamblePanel;
    [SerializeField] private GameObject MarkersScannedPanel;
    [SerializeField] private GameObject RefImagePanel;

    [Header("Bounding Box")]
    [SerializeField] private GameObject BoundingBox;

    // other variables that do not need input
    private GameObject KokoKrunchMarker, SafeguardMarker, PringlesMarker, CarMarker, StonesMarker;
    private float StartTime;
    private bool IsSolved, GameStart, TimerIsRunning, AllMarkersScanned, HasPlayed;
    private int Counter, NumberOfMarkers, MarkersLeft;
    private List<float> Distances, AnchorDist, AngleX, AngleY, AngleZ, Rotations, Delta;
    private List<float> GoodDistance, GoodAnchorDist, GoodRotation, GoodX, GoodY, GoodZ, GoodDelta;
    private List<bool> DistanceCheck, AnchorDistCheck, RotationCheck, XCheck, YCheck, ZCheck, DeltaCheck;
    private List<GameObject> MarkersList, SolutionMarkersList;
    private List<Transform> PuzzlePieces;
    private List<string> MarkersFound;


    /* ====================================================================== */


    private void Start()
    {
        Confetti.Stop();
        Confetti.Clear();

        MarkersFound = new List<string>();
        MarkersList = new List<GameObject>();
        PuzzlePieces = new List<Transform>();
        SolutionMarkersList = new List<GameObject>();
        GetChildrenFromParentModel();

        GoodDistance = new List<float>();
        GoodAnchorDist = new List<float>();
        GoodRotation = new List<float>();
        GoodX = new List<float>();
        GoodY = new List<float>();
        GoodZ = new List<float>();
        GoodDelta = new List<float>();

        WholeModels.SetActive(false);
        Model.SetActive(false);

        BoundingBox.SetActive(false);
        ResultPanel.SetActive(false);

        HasPlayed = false;
        TimerIsRunning = false;
        AllMarkersScanned = false;
        GameStart = false;
        NumberOfMarkers = 0;
        Counter = 0;
    }


    
    private void StartGame()
    {
        AllMarkersScanned = false;
        GameStart = true;
        IsSolved = false;
        MarkersLeft = NumberOfMarkers;

        DistributePuzzlePieces();
        Destroy(Model);
    }


    private void Update()
    {
        if (GameStart && (Counter >= NumberOfMarkers))
        {
            if (IsSolved)
            {
                return;
            }
            TimeLapseCounter();
            UpdateDistanceTracker();
            UpdateAnchorDistTracker();
            UpdateRotationTracker();
            UpdateDeltaTracker();
            DetermineIfSolved();
        }
    }



    private void TimeLapseCounter()
    {
        if (TimerIsRunning)
        {
            UpdateTimeLapseCounter();
        }
        else
        {
            StartTimeLapseCounter();
        }
    }


    private void StartTimeLapseCounter()
    {
        if (TimerIsRunning == false)
        {
            TimerIsRunning = true;
        }
        StartTime = Time.time;
    }


    private void UpdateTimeLapseCounter()
    {
        float currentTime = Time.time - StartTime;
        string timeMinutes = ((int)currentTime / 60).ToString();
        string timeSeconds = (currentTime % 60).ToString("f2");

        // Update UI texts time lapse
        TimeLapsedText.text = "Time Lapsed: " + timeMinutes + ":" + timeSeconds;
        ResultTimeText.text = "Time Lapsed: " + timeMinutes + ":" + timeSeconds;
    }



    private void UpdateDistanceTracker()
    {
        Distances = new List<float>();
        string distString = "";

        for (int i = 0; i < MarkersList.Count; i++)
        {
            Distances.Add(Vector3.Distance(ARCamera.transform.position, MarkersList[i].transform.position));

            if (Distances[i] > (GoodDistance[i] - ErrorDistance) && Distances[i] < (GoodDistance[i] + ErrorDistance))
            {
                distString = distString + "<color=green>" + Distances[i].ToString("f2") + "</color>" + "; ";
            }
            else
            {
                distString = distString + "<color=red>" + Distances[i].ToString("f2") + "</color>" + "; ";
            }
        }

        // TESTING: update UI text for distances
        DistanceText.text = "DISTANCE: " + distString;
    }


    private void UpdateAnchorDistTracker()
    {
        AnchorDist = new List<float>();
        string anchorDistString = "";

        for (int i = 0; i < MarkersList.Count; i++)
        {
            AnchorDist.Add(Vector3.Distance(MarkersList[i].transform.position, SolutionMarkersList[i].transform.position));
            if (AnchorDist[i] > (GoodAnchorDist[i] - ErrorAnchorDist) && AnchorDist[i] < (GoodAnchorDist[i] + ErrorAnchorDist))
            {
                anchorDistString = anchorDistString + "<color=green>" + AnchorDist[i].ToString("f2") + "</color>" + "; ";
            }
            else
            {
                anchorDistString = anchorDistString + "<color=red>" + AnchorDist[i].ToString("f2") + "</color>" + "; ";
            }
        }

        // TESTING: update UI text for distances
        AnchorDistText.text = "ANCHOR DISTANCE: " + anchorDistString;
    }


    private void UpdateRotationTracker()
    {
        Rotations = new List<float>();
        string rotString = "";

        for (int i = 0; i < MarkersList.Count; i++)
        {
            float angle1 = Vector3.Angle(SolutionMarkersList[i].transform.forward, MarkersList[i].transform.position - SolutionMarkersList[i].transform.position);
            float angle2 = Vector3.Angle(SolutionMarkersList[i].transform.right, MarkersList[i].transform.position - SolutionMarkersList[i].transform.position);

            if (angle2 < 180)
            {
                angle1 = 360 - angle1;
            }

            Rotations.Add(angle1);

            if (Rotations[i] > (GoodRotation[i] - ErrorRotation) && Rotations[i] < (GoodRotation[i] + ErrorRotation))
            {
                rotString = rotString + "<color=green>" + Rotations[i].ToString("f2") + "</color>" + "; ";
            }
            else
            {
                rotString = rotString + "<color=red>" + Rotations[i].ToString("f2") + "</color>" + "; ";
            }
        }

        // TESTING: updated UI text for rotations
        RotationText.text = "ROTATION: " + rotString;
    }

    private void UpdateDeltaTracker()
    {
        Delta = new List<float>();
        string deltaString = "";

        for (int i = 0; i < MarkersList.Count; i++)
        {
            float angle1 = Vector3.Angle(ARCamera.transform.forward, MarkersList[i].transform.position - ARCamera.transform.position);
            float angle2 = Vector3.Angle(ARCamera.transform.right, MarkersList[i].transform.position - ARCamera.transform.position);

            if (angle2 < 180)
            {
                angle1 = 360 - angle1;
            }

            Delta.Add(angle1);

            if (Delta[i] > (GoodDelta[i] - DeltaError) && Delta[i] < (GoodDelta[i] + DeltaError))
            {
                deltaString = deltaString + "<color=green>" + Delta[i].ToString("f2") + "</color>" + "; ";
            }
            else
            {
                deltaString = deltaString + "<color=red>" + Delta[i].ToString("f2") + "</color>" + "; ";
            }
        }

        // TESTING: update UI for delta angles
        DeltaText.text = "DELTA: " + deltaString;
    }



    private void DetermineIfSolved()
    {
        DistanceCheck = new List<bool>();
        AnchorDistCheck = new List<bool>();
        RotationCheck = new List<bool>();
        XCheck = new List<bool>();
        YCheck = new List<bool>();
        ZCheck = new List<bool>();
        DeltaCheck = new List<bool>();

        for (int i = 0; i < MarkersList.Count; i++)
        {
            DistanceCheck.Add(Distances[i] > (GoodDistance[i] - ErrorDistance) && Distances[i] < (GoodDistance[i] + ErrorDistance));
            AnchorDistCheck.Add(AnchorDist[i] > (GoodAnchorDist[i] - ErrorAnchorDist) && AnchorDist[i] < (GoodAnchorDist[i] + ErrorAnchorDist));
            RotationCheck.Add(Rotations[i] > (GoodRotation[i] - ErrorRotation) && Rotations[i] < (GoodRotation[i] + ErrorRotation));
            DeltaCheck.Add(Delta[i] > (GoodDelta[i] - DeltaError) && Delta[i] < (GoodDelta[i] + DeltaError));
        }

        bool resultDistance = !DistanceCheck.Contains(false);
        bool resultAnchorDist = !AnchorDistCheck.Contains(false);
        bool resultRotation = !RotationCheck.Contains(false);
        bool resultDelta = !DeltaCheck.Contains(false);

        if (MarkersLeft > 1)
        {
            GameObject gameObjectPieces;

            for (int index = 0; index < MarkersList.Count; index++)
            {
                if (DistanceCheck[index] && AnchorDistCheck[index] && RotationCheck[index] && DeltaCheck[index])
                {
                    if (MarkersList[index].name == "KokoKrunchMarker" || MarkersList[index].name == "SafeguardMarker")
                    {
                        try
                        {
                            if (MarkersList[index].transform.GetChild(1).gameObject.name.Contains("Pieces"))
                            {
                                GameObject dummyObject = new GameObject("DummyObject");
                                dummyObject.transform.parent = MarkersList[index].transform;

                                // if the box markers are in the correct position
                                // distribute the subcontainerPieces to image marker?
                                gameObjectPieces = MarkersList[index].transform.GetChild(1).gameObject;
                                gameObjectPieces.transform.parent = MarkersList[0].transform;
                                MarkersLeft--;
                                
                                //DistributeToOneMarker(StonesMarker, SolutionMarker4, gameObjectPieces);
                            }
                        } catch (MissingReferenceException e)
                        {
                            Debug.Log(e);
                        }
                        
                    }
                    else if (MarkersList[index].name == "PringlesMarker" || MarkersList[index].name == "CarMarker")
                    {
                        //// if the cylinder and car markers are in the correct position
                        //foreach (Transform pieces in MarkersList[index].transform.GetChild(0))
                        //{
                        //    pieces.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        //}

                        try
                        {
                            if (MarkersList[index].transform.GetChild(0).gameObject.name.Contains("Pieces"))
                            {
                                GameObject dummyObject = new GameObject("DummyObject");
                                dummyObject.transform.parent = MarkersList[index].transform;

                                // if the box markers are in the correct position
                                // distribute the subcontainerPieces to image marker?
                                gameObjectPieces = MarkersList[index].transform.GetChild(0).gameObject;
                                gameObjectPieces.transform.parent = MarkersList[0].transform;
                                MarkersLeft--;

                                //DistributeToOneMarker(StonesMarker, SolutionMarker4, gameObjectPieces);
                            }
                        }
                        catch (MissingReferenceException e)
                        {
                            Debug.Log(e);
                        }
                    }
                    else
                    {
                        // if the image marker are in the correct position
                        foreach (Transform pieces in MarkersList[index].transform.GetChild(3))
                        {
                            pieces.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                }
                else
                {
                    if (MarkersList[index].name == "KokoKrunchMarker" || MarkersList[index].name == "SafeguardMarker")
                    {
                        foreach (Transform pieces in MarkersList[index].transform.GetChild(1))
                        {
                            pieces.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }
                    }
                    else if (MarkersList[index].name == "PringlesMarker" || MarkersList[index].name == "CarMarker")
                    {
                        foreach (Transform pieces in MarkersList[index].transform.GetChild(0))
                        {
                            pieces.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }
                    }
                    else
                    {
                        foreach (Transform pieces in MarkersList[index].transform.GetChild(3))
                        {
                            pieces.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        }
                    }
                }
            }
        }


        if ((Distances[0] > (GoodDistance[0] - ErrorDistance) && Distances[0] < (GoodDistance[0] + ErrorDistance)) &&
            (AnchorDist[0] > (GoodAnchorDist[0] - ErrorAnchorDist) && AnchorDist[0] < (GoodAnchorDist[0] + ErrorAnchorDist)) &&
            (Rotations[0] > (GoodRotation[0] - ErrorRotation) && Rotations[0] < (GoodRotation[0] + ErrorRotation)) &&
            (Delta[0] > (GoodDelta[0] - DeltaError) && Delta[0] < (GoodDelta[0] + DeltaError)) && (MarkersLeft == 1))
        {
            PuzzleSolved();
        }
    }


    
    private void PuzzleSolved()
    {
        IsSolved = true;

        // disable texts for testing
        DistanceText.enabled = false;
        AnchorDistText.enabled = false;
        RotationText.enabled = false;
        AngleText.enabled = false;
        DeltaText.enabled = false;

        // display confetti
        PlayConfetti();

        //  set children of remaining markers as visible = false
        for (int index = 3; index < MarkersList[0].transform.childCount; index++)
        {
            Debug.Log(MarkersList[0].transform.GetChild(index).gameObject.name);
            MarkersList[0].transform.GetChild(index).gameObject.SetActive(false);
        }

        // set whole models as visible
        WholeModels.SetActive(true);

        // show results panel
        StartCoroutine(ShowWholeModels(1));
    }


    private IEnumerator ShowWholeModels(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        OpenResultsPanel();
    }


    private void PlayConfetti()
    {
        // display confetti
        if (!HasPlayed)
        {
            Confetti.Play();
            Destroy(Confetti, 5);
            HasPlayed = true;
        }
    }


    public void FindGameObjectMarker(string marker)
    {
        // Koko Krunch Marker (1)
        if (marker.Contains("KokoKrunchMarker"))
        {
            KokoKrunchMarker = GameObject.Find(marker);
            MarkersList.Add(KokoKrunchMarker);

            SolutionMarkersList.Add(SolutionMarker1);
            GoodDistance.Add(CorrectDistance1);
            GoodAnchorDist.Add(CorrectAnchorDist1);
            GoodRotation.Add(CorrectRotation1);
            GoodDelta.Add(DeltaValue1);
        }

        // Safeguard Marker (2)
        else if (marker.Contains("SafeguardMarker"))
        {
            SafeguardMarker = GameObject.Find(marker);
            MarkersList.Add(SafeguardMarker);

            SolutionMarkersList.Add(SolutionMarker2);
            GoodDistance.Add(CorrectDistance2);
            GoodAnchorDist.Add(CorrectAnchorDist2);
            GoodRotation.Add(CorrectRotation2);
            GoodDelta.Add(DeltaValue2);
        }

        // Pringles Marker (3)
        else if (marker.Contains("PringlesMarker"))
        {
            PringlesMarker = GameObject.Find(marker);
            MarkersList.Add(PringlesMarker);

            SolutionMarkersList.Add(SolutionMarker3);
            GoodDistance.Add(CorrectDistance3);
            GoodAnchorDist.Add(CorrectAnchorDist3);
            GoodRotation.Add(CorrectRotation3);
            GoodDelta.Add(DeltaValue3);
        }

        // Stones Marker (4)
        else if (marker.Contains("StonesMarker"))
        {
            StonesMarker = GameObject.Find(marker);
            MarkersList.Insert(0,StonesMarker);

            SolutionMarkersList.Insert(0, SolutionMarker4);
            GoodDistance.Insert(0, CorrectDistance4);
            GoodAnchorDist.Insert(0, CorrectAnchorDist4);
            GoodRotation.Insert(0, CorrectRotation4);
            GoodDelta.Insert(0, DeltaValue4);
        }

        // Blue Car Marker (5)
        else if (marker.Contains("CarMarker"))
        {
            CarMarker = GameObject.Find(marker);
            MarkersList.Add(CarMarker);

            SolutionMarkersList.Add(SolutionMarker5);
            GoodDistance.Add(CorrectDistance5);
            GoodAnchorDist.Add(CorrectAnchorDist5);
            GoodRotation.Add(CorrectRotation5);
            GoodDelta.Add(DeltaValue5);
        }
    }


    private void DistributePuzzlePieces()
    {
        DistributeToMultipleMarkers(MarkersList, SolutionMarkersList);
    }



    private void DistributeToOneMarker(GameObject marker, GameObject solution, GameObject Pieces)
    {
        Pieces.transform.SetParent(solution.transform);
        Pieces.transform.localScale = Model.transform.localScale;
        Pieces.transform.position = Model.transform.position;
        Pieces.transform.SetParent(marker.transform, false);
    }


    private void DistributeToMultipleMarkers(List<GameObject> markers, List<GameObject> solutions)
    {
        Debug.Log("SIZE: " + solutions.Count);

        int max = (PuzzlePieces.Count - 1) / NumberOfMarkers;
        int mrkCnt = NumberOfMarkers;
        List<GameObject> PuzzleHolders = new List<GameObject>();
        int[] distribution = new int[mrkCnt];
        for (int i = 0; i < mrkCnt; i++)
        {
            distribution[i] = 0;
            PuzzleHolders.Add(new GameObject("Pieces" + i));
            PuzzleHolders[i].transform.parent = Model.transform;
            PuzzleHolders[i].transform.localPosition = new Vector3(0, 0, 0);
            PuzzleHolders[i].transform.localEulerAngles = Model.transform.localEulerAngles;
        }
        for (int i = 0; i < PuzzlePieces.Count; i++)
        {
            int rnd = Random.Range(0, mrkCnt);

            if (distribution[rnd] > max)
            {
                for (int j = 0; j < mrkCnt; j++)
                {
                    if (distribution[j] <= max)
                        rnd = j;
                }
            }

            PuzzlePieces[i].SetParent(PuzzleHolders[rnd].transform, false);

            distribution[rnd]++;
        }

        for (int i = 0; i < mrkCnt; i++)
        {
            DistributeToOneMarker(markers[i], solutions[i], PuzzleHolders[i]);
        }
    }



    private void GetChildrenFromParentModel()
    {
        Transform[] pieces = Model.transform.GetComponentsInChildren<Transform>(true);
        for (int index = 0; index < pieces.Length; index++)
        {
            PuzzlePieces.Add(pieces[index]);
            //Debug.Log(PuzzlePieces[index].name);
        }
    }


    public void GetNumberOfMarkers(string number)
    {
        NumberOfMarkers = int.Parse(number);
        Debug.Log("NUMBER OF MARKERS: " + number);
    }



    public void OnTargetFound(string name)
    {
        string markersfound = "";
        Debug.Log(name);

        if (PreamblePanel.activeInHierarchy == false)
        {
            if (MarkersFound.Count <= NumberOfMarkers && Counter <= NumberOfMarkers && GameStart == false)
            {
                if (!MarkersFound.Contains(name))
                {
                    FindGameObjectMarker(name);
                    if(name.Contains("StonesMarker"))
                    {
                        MarkersFound.Insert(0, name);
                    }
                    else
                    {
                        MarkersFound.Add(name);
                    }
                    Counter++;

                    Debug.Log("COUNTER: " + Counter);

                    TargetsFoundText.text = "MARKERS FOUND: " + MarkersFound.Count.ToString() + "/" + NumberOfMarkers.ToString();


                    for (int i = 0; i < MarkersFound.Count; i++)
                    {
                        markersfound = markersfound + MarkersFound[i] + "; ";
                    }

                    MarkersListText.text = markersfound;
                }
                if (MarkersFound.Count == NumberOfMarkers && GameStart == false)
                {
                    StartCoroutine(WaitForAllMarkers());
                }
            }
        }
    }


    IEnumerator WaitForAllMarkers()
    {
        yield return new WaitUntil(() => MarkersFound.Count == NumberOfMarkers);

        if (MarkersFound.Count == NumberOfMarkers)
        {
            AllMarkersScanned = true;
        }

        OpenMarkersScannedPanel();
    }

    // done
    private void OpenResultsPanel()
    {
        ResultPanel.SetActive(true);
    }

    
    public void OpenCloseBoundingBox()
    {
        if (BoundingBox.activeSelf)
        {
            BoundingBox.SetActive(false);
        }
        else
        {
            BoundingBox.SetActive(true);
        }
    }

    // done
    public void ClosePreamblePanel()
    {
        PreamblePanel.SetActive(false);
    }

    // done
    private void OpenMarkersScannedPanel()
    {
        if (MarkersFound.Count == NumberOfMarkers && AllMarkersScanned == true)
        {
            MarkersScannedPanel.SetActive(true);
        }
    }

    // done
    public void CloseMarkersScannedPanel()
    {
        MarkersScannedPanel.SetActive(false);
        StartGame();
    }

    public void OpenCloseRefImagePanel()
    {
        if (RefImagePanel.activeSelf)
        {
            RefImagePanel.SetActive(false);
        }
        else
        {
            RefImagePanel.SetActive(true);
        }
    }


    public void EnableAndDisableTexts()
    {
        if (DistanceText.gameObject.activeSelf)
        {
            DistanceText.gameObject.SetActive(false);
            AnchorDistText.gameObject.SetActive(false);
            RotationText.gameObject.SetActive(false);
            //AngleText.gameObject.SetActive(false);
            DeltaText.gameObject.SetActive(false);
        }
        else
        {
            DistanceText.gameObject.SetActive(true);
            AnchorDistText.gameObject.SetActive(true);
            RotationText.gameObject.SetActive(true);
            //AngleText.gameObject.SetActive(true);
            DeltaText.gameObject.SetActive(true);
        }
    }
}
