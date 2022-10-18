using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CreateLocationEditor : EditorWindow
{
    private List<SoldierGroup> soldiers = new();

    private TextField _locationName;
    private EnumField _alignment;
    private ListView _garrsionList;

    private string _pathToTargetDirectory = "UNKNOWN";

    [MenuItem("Assets/Create/Game/Location with army")]
    public static void ShowExample()
    {
        CreateLocationEditor wnd = GetWindow<CreateLocationEditor>();
        wnd.titleContent = new GUIContent("Add location");
    }

    private void Awake()
    {
        _pathToTargetDirectory = LamTools.EditorUtility.GetCurrentEditorDirectory();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        var generalLabel = new Label("General");
        generalLabel.style.alignSelf = Align.Center;
        generalLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(generalLabel);

        _locationName = new TextField("Location Name");
        _locationName.value = _pathToTargetDirectory.Split('/').Last();

        _alignment = new EnumField("Alignment", Alignment.Human);

        root.Add(_locationName);
        root.Add(_alignment);

        var armyLabel = new Label("Army");
        armyLabel.style.alignSelf = Align.Center;
        armyLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        root.Add(armyLabel);

        _garrsionList = new ListView(soldiers, -1, CreateSoldierGroupView, FillSoldiers);
        _garrsionList.headerTitle = "Garrison";
        _garrsionList.showAddRemoveFooter = true;
        _garrsionList.showFoldoutHeader = true;
        root.Add(_garrsionList);


        var createButton = new Button(ConstructElements);
        createButton.text = $"Create in {_pathToTargetDirectory}";
        root.Add(createButton);
    }


    private VisualElement CreateSoldierGroupView()
    {
        var group = new Box();
        group.style.alignItems = Align.FlexStart;
        group.style.flexDirection = FlexDirection.Row;

        var numberOfSoldiersInput = new IntegerField();
        numberOfSoldiersInput.maxLength = 3;
        group.Add(new IntegerField());


        var picker = new ObjectField()
        {
            allowSceneObjects = false,
            objectType = typeof(UnitData),
        };
        picker.style.maxWidth = 400;

        group.Add(picker);

        return group;
    }

    private void FillSoldiers(VisualElement root, int index)
    {
        if (soldiers[index] == null)
        {
            soldiers[index] = new SoldierGroup();
        }

        var group = soldiers[index];

        var numberInput = root.Q<IntegerField>();
        numberInput.value = group.NumberOfMembers;
        numberInput.RegisterValueChangedCallback(valueChangedEvent => group.NumberOfMembers = valueChangedEvent.newValue);

        var typeInput = root.Q<ObjectField>();
        typeInput.value = group.unitData;
        typeInput.RegisterValueChangedCallback(valueChangedEvent => group.unitData = (UnitData)valueChangedEvent.newValue);
    }


    private void ConstructElements()
    {
        var locationName = _locationName.value;

        if(_pathToTargetDirectory == "UNKNOWN")
        {
            EditorUtility.DisplayDialog("Error", "Invalid asset directory.", "Ok");
            Close();
            return;
        }

        if (locationName == "")
        {
            EditorUtility.DisplayDialog("Error", "Location cannot have empty name.", "Ok");
            return;
        }

        if(!soldiers.TrueForAll(x => x.NumberOfMembers > 0 && x.unitData != null))
        {
            EditorUtility.DisplayDialog("Error", "Garrison must have all fields set.", "Ok");
            return;
        }

        var army = CreateInstance<Army>();
        army.ArmyName = "Garrison";
        army.name = $"{locationName}'s Garrison";
        army.soldiers = soldiers;
        army.Aligment = (Alignment)_alignment.value;

        var location = CreateInstance<LocationData>();
        location.LocationName = locationName;
        location.name = locationName;
        location.Garrison = army;
        location.alignment = (Alignment)_alignment.value;


        AssetDatabase.CreateAsset(army, $"{_pathToTargetDirectory}/{locationName} Garrison.asset");
        AssetDatabase.CreateAsset(location, $"{_pathToTargetDirectory}/{locationName}.asset");

        Close();
    }

}