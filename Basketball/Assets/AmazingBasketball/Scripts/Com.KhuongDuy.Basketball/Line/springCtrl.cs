using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class springCtrl : MonoBehaviour
{
    // Start is called before the first frame update

    private SpringJoint2D[] m_sprintJoints;
    private Rigidbody2D m_connectionRigidbody;

    private LineRenderer m_lineRender;
    void Start()
    {
        m_sprintJoints = (SpringJoint2D[])GetComponents<SpringJoint2D>();
        m_lineRender = (LineRenderer)GetComponent<LineRenderer>();
        if(m_sprintJoints.Length>0 && m_lineRender)
        {
            m_lineRender.SetVertexCount(m_sprintJoints.Length*2);
        }

        var spriteRender = (SpriteRenderer)GetComponent<SpriteRenderer>();
        Destroy(spriteRender);
    }

    // Update is called once per frame
    void Update()
    {
        int n=0;
        for(int i = 0;  i < m_sprintJoints.Length; ++i)
        {
            var connRigidbody = m_sprintJoints[i].connectedBody;
            if(connRigidbody)
            {
                m_lineRender.SetPosition (n++, transform.position);
                m_lineRender.SetPosition (n++, connRigidbody.transform.position);
            }
        }
    }
}
