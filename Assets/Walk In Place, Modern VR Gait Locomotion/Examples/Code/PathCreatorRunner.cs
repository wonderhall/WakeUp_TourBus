using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// Moves along a path at the gait speed.
[RequireComponent(typeof(GaitLocomotion))]
public class PathCreatorRunner : MonoBehaviour
{
    #region member variables

    public PathCreator m_path;

    private GaitLocomotion m_locomotion;
    private float m_distance;

    #endregion

    void Start()
    {
        m_locomotion = GetComponent<GaitLocomotion>();

        if (m_path != null)
        {
            m_distance = m_path.path.GetClosestDistanceAlongPath(transform.position);
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            m_path.pathUpdated += OnPathChanged;
        }
    }

    void Update()
    {
        if (m_path != null)
        {
            m_distance += m_locomotion.GetSpeed();
            transform.position = m_path.path.GetPointAtDistance(m_distance, EndOfPathInstruction.Loop);
            Vector3 rot = m_path.path.GetRotationAtDistance(m_distance, EndOfPathInstruction.Loop).eulerAngles;
            rot.Scale(new Vector3(0, 1, 0));
            transform.rotation = Quaternion.Euler(rot);
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        m_distance = m_path.path.GetClosestDistanceAlongPath(transform.position);
    }
}
