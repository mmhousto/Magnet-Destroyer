using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using Vector2 = UnityEngine.Vector2;

#region DisableAttribute

[AttributeUsage(AttributeTargets.Field)]
public class NotActiveIf : PropertyAttribute
{
    public string Target;
    public bool DisabledState;

    public NotActiveIf(string targetName, bool state)
    {
        Target = targetName;
        DisabledState = state;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NotActiveIf))]
public class DisableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty p, GUIContent label)
    {
        NotActiveIf a = (NotActiveIf)attribute;
        SerializedProperty t = p.serializedObject.FindProperty(a.Target);

        bool newState = true;
        bool oldState = GUI.enabled;

        if (t == null) Debug.LogWarning("Invalid Name", p.serializedObject.targetObject);
        else newState = t.boolValue != a.DisabledState;

        GUI.enabled = newState;
        EditorGUI.PropertyField(position, p, label, true);
        GUI.enabled = oldState;
    }
}
#endif
#endregion

public class MagneticTool2D : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] [Tooltip("When you want it to be affected by magnetism but it is not a magnetic object")] private bool isMetallic;
    [SerializeField] [Tooltip("When you don't want it to move")] private bool isStatic;

    [Space]
    [SerializeField] [Tooltip("Is more accurate with forces")] private bool magnetismWithoutUseForces;
    [SerializeField, NotActiveIf("magnetismWithoutUseForces", true)] [Tooltip("If you do not put anything, the mass will be assumed to be one KG")] private Rigidbody2D magneticRigidbody;
    [Space]
    [SerializeField] private bool AdvanceSettings;
    
    [Space]
    [SerializeField] private bool turnOnMagnetism = true;
    [SerializeField] private bool affectByMagnetism = true;

    [Header("Magnetic Options")]
    [SerializeField, NotActiveIf("isMetallic", true)] private bool northPole;

    [SerializeField, NotActiveIf("isMetallic", true)] [Tooltip("If the magnetic force is kept constant in the distance")] private bool constantMagneticForce;

    [Header("Magnetic Force")]
    [SerializeField, NotActiveIf("isMetallic", true)] private float magneticForce;
    [SerializeField, NotActiveIf("isMetallic", true)] private float magneticDistance;

    [Header("Axis block Movement")]
    [SerializeField] private bool X_Axis;
    [SerializeField] private bool Y_Axis;

    [Header("Tags")]
    [SerializeField] [Tooltip("Magnetism only affects objects with these tags")] private List<String> allowTags;

    [Header("Advance Settings")]
    [SerializeField, NotActiveIf("AdvanceSettings", false)] private bool intermittentShutdown;
    [SerializeField, NotActiveIf("intermittentShutdown", false)] private float OffsetTime;
    [SerializeField, NotActiveIf("intermittentShutdown", false)] private float TimeOn;
    [SerializeField, NotActiveIf("intermittentShutdown", false)] private float TimeOff;

    [Space]
    [SerializeField, NotActiveIf("AdvanceSettings", false)] private bool changePolarity;
    [SerializeField, NotActiveIf("changePolarity", false)] private float timeToChange;

    #endregion

    #region Other Variables
    private const float MAGNETIC_CONS = 0.1f;
    private const float PI = 3.1415f;
    private bool offsetDone;
    private CircleCollider2D magneticArea;
    private List<GameObject> magneticObjects;
    private List<GameObject> magneticObjectsOut;
    private float timer;
    private float polarityTimer;
    #endregion

    #region Setters/Getters
    public bool IsMetallic { get => isMetallic; set => isMetallic = value; }
    public bool IsStatic { get => isStatic; set => isStatic = value; }
    public bool NorthPole { get => northPole; set => northPole = value; }
    public bool ConstantMagneticForce { get => constantMagneticForce; set => constantMagneticForce = value; }
    public float MagneticCharge { get => magneticForce; set => magneticForce = value; }
    public float MagneticDistance { get => magneticDistance; set => magneticDistance = value; }
    public bool TurnOnMagnetism { get => turnOnMagnetism; set => turnOnMagnetism = value; }
    public bool AffectByMagnetism { get => affectByMagnetism; set => affectByMagnetism = value; }
    public bool XAxis { get => X_Axis; set => X_Axis = value; }
    public bool YAxis { get => Y_Axis; set => Y_Axis = value; }
    public List<string> AllowTags { get => allowTags; set => allowTags = value; }
    #endregion

    #region Unity Functions
    void Start()
    {
        offsetDone = false;
        magneticObjects = new List<GameObject>();
        magneticObjectsOut = new List<GameObject>();
        magneticArea = gameObject.AddComponent<CircleCollider2D>();
        magneticArea.isTrigger = true;
        magneticArea.radius = magneticDistance;
        timer = 0;
        polarityTimer = 0;
    }

    private void Update()
    {
        if (magneticArea.radius != magneticDistance)
        {
            magneticArea.radius = magneticDistance;
        }

        if (AdvanceSettings)
        {
            timer += Time.deltaTime;
            polarityTimer += Time.deltaTime;

            if (intermittentShutdown)
            {
                if (offsetDone)
                {
                    if (turnOnMagnetism)
                    {
                        if (timer >= TimeOn)
                        {
                            turnOnMagnetism = false;
                            timer = 0;
                        }
                        else
                        {
                            turnOnMagnetism = true;
                        }
                    }
                    else
                    {
                        if (timer >= TimeOff)
                        {
                            turnOnMagnetism = true;
                            timer = 0;
                        }
                        else
                        {
                            turnOnMagnetism = false;
                        }
                    }
                }
                else
                {
                    if (timer >= OffsetTime)
                    {
                        timer = 0;
                        offsetDone = true;
                        turnOnMagnetism = true;
                    }
                }
            }

            if (changePolarity)
            {
                if (polarityTimer >= timeToChange)
                {
                    northPole = !northPole;
                    polarityTimer = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMetallic || !turnOnMagnetism) return;

        //we put the mass
        float mass = 0;
        if (!magneticRigidbody) mass = 1;
        else mass = magneticRigidbody.mass;

        foreach (var magneticObject in magneticObjects)
        {
            if (magneticObject == null) continue;
            float distance = Vector2.Distance(magneticObject.transform.position, transform.position);

            var magneticScript = magneticObject.GetComponent<MagneticTool2D>();
            if (magneticScript.isStatic) continue;
            if (!magneticScript.affectByMagnetism) continue;

            float Force = 0;
            //we calculate the force
            if (!constantMagneticForce)
            {
                Force = MAGNETIC_CONS * (magneticForce * mass) / (distance * distance);
            }
            else Force = MAGNETIC_CONS * magneticForce * mass;

            Vector2 vectorDirection = transform.position - magneticObject.transform.position;
            Vector2 vectorDirectionNorm = vectorDirection.normalized;
            Vector2 objectForce = Vector2.zero;

            if (!magnetismWithoutUseForces)
            {
                //attract or not the object
                if (northPole != magneticScript.northPole || magneticScript.isMetallic)
                {
                    if (!magneticScript.XAxis) objectForce.x = vectorDirectionNorm.x * Force;
                    if (!magneticScript.YAxis) objectForce.y = vectorDirectionNorm.y * Force;
                }
                else
                {
                    if (!magneticScript.XAxis) objectForce.x = -vectorDirectionNorm.x * Force;
                    if (!magneticScript.YAxis) objectForce.y = -vectorDirectionNorm.y * Force;
                }

                magneticObject.GetComponent<Rigidbody2D>().AddForceAtPosition(objectForce, magneticObject.transform.position, ForceMode2D.Force);
            }
            else
            {
                Force /= 250;
                var position = magneticScript.transform.position;
                if (northPole != magneticScript.northPole || magneticScript.isMetallic)
                {
                    if (!magneticScript.XAxis) position.x += vectorDirectionNorm.x * Force;
                    if (!magneticScript.YAxis) position.y += vectorDirectionNorm.y * Force;
                }
                else
                {
                    if (!magneticScript.XAxis) position.x -= vectorDirectionNorm.x * Force;
                    if (!magneticScript.YAxis) position.y -= vectorDirectionNorm.y * Force;
                }

                magneticScript.transform.position = position;
            }
        }

        for (int i = 0; i < magneticObjectsOut.Count; i++)
        {
            var magneticObject = magneticObjectsOut[i];
            if (magneticObject == null) continue;
            float distance = Vector2.Distance(magneticObject.transform.position, transform.position);

            var magneticScript = magneticObject.GetComponent<MagneticTool2D>();
            if (magneticScript.isStatic) continue;
            if (!magneticScript.affectByMagnetism) continue;

            float Force = 0;
            //we calculate the force
             Force = MAGNETIC_CONS * (magneticForce * mass) / (distance * distance);

            if (Force < 0.01)
            {
                magneticObjectsOut.Remove(magneticObject);
                i--;
                continue;
            }

            Vector2 vectorDirection = transform.position - magneticObject.transform.position;
            Vector2 vectorDirectionNorm = vectorDirection.normalized;

            Force /= 250;
            var position = magneticScript.transform.position;
            if (northPole != magneticScript.northPole || magneticScript.isMetallic)
            {
                if (!magneticScript.XAxis) position.x += vectorDirectionNorm.x * Force;
                if (!magneticScript.YAxis) position.y += vectorDirectionNorm.y * Force;
            }
            else
            {
                if (!magneticScript.XAxis) position.x -= vectorDirectionNorm.x * Force;
                if (!magneticScript.YAxis) position.y -= vectorDirectionNorm.y * Force;
            }

            magneticScript.transform.position = position;
        }
    }

    #endregion

    #region Triggers

    //Add every magnetic object inside an area
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!TagAllow(col.tag)) return;
        if (magneticObjects.Contains(col.gameObject) || col.gameObject == gameObject || !turnOnMagnetism) return;

        var magneticTool = col.gameObject.GetComponent<MagneticTool2D>();

        if (magneticTool)
        {
           magneticObjects.Add(col.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (magneticObjects.Contains(col.gameObject) || col.gameObject == gameObject || !turnOnMagnetism) return;
        if (!TagAllow(col.tag)) return;

        var magneticTool = col.gameObject.GetComponent<MagneticTool2D>();

        if (magneticTool)
        {
            magneticObjects.Add(col.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!magneticObjects.Contains(col.gameObject)) return;

        if (col.gameObject.GetComponent<MagneticTool2D>())
        {
            magneticObjects.Remove(col.gameObject);
            if(magnetismWithoutUseForces && !magneticObjectsOut.Contains(col.gameObject) && !constantMagneticForce) magneticObjectsOut.Add(col.gameObject);
        }
    }

    #endregion

    #region Other functions
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (!northPole) Gizmos.color = Color.red;
        else Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, magneticDistance);
    }
#endif
    private bool TagAllow(string tag)
    {
        if (allowTags.Count <= 0) return true;

        if (allowTags.Contains(""))
        {
            for (int i = allowTags.Count - 1; i >= 0; i--)
            {
                if (allowTags[i] == "") allowTags.RemoveAt(i);
            }
        }

        for (int i = 0; i < allowTags.Count; i++)
        {
            if (allowTags[i] == "") continue;
            if (allowTags[i] == tag) return true;
        }

        return false;
    }

    #endregion
}
