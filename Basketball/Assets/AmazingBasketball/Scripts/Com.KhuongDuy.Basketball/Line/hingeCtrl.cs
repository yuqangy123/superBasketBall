using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hingeCtrl : MonoBehaviour
{
    // Start is called before the first frame update

    private HingeJoint2D[] m_hingeJoints;

    private LineRenderer m_lineRender;
    void Start()
    {
        m_hingeJoints = (HingeJoint2D[])GetComponents<HingeJoint2D>();
        m_lineRender = (LineRenderer)GetComponent<LineRenderer>();
        if(m_hingeJoints.Length>0 && m_lineRender)
        {
            m_lineRender.SetVertexCount(m_hingeJoints.Length*2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int n=0;
        for(int i = 0;  i < m_hingeJoints.Length; ++i)
        {
            var connRigidbody = m_hingeJoints[i].connectedBody;
            if(connRigidbody)
            {
                m_lineRender.SetPosition (n++, transform.position);
                m_lineRender.SetPosition (n++, connRigidbody.transform.position);
            }
        }
    }
}

