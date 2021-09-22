using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class PlayableTest : MonoBehaviour
{
    public AnimationClip m_animClip;
    private PlayableGraph m_graph;

    void Start()
    {
        m_graph = PlayableGraph.Create("playable animation test");
        var animOutput = AnimationPlayableOutput.Create(m_graph, "output", GetComponent<Animator>());
        var animClip = AnimationClipPlayable.Create(m_graph, m_animClip);
        animOutput.SetSourcePlayable(animClip);

        m_graph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
 
    }
    
    void OnDestroy()
    {
        m_graph.Destroy();
    }
}
