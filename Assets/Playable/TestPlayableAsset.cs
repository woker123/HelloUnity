using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TestPlayableAsset : PlayableAsset
{
    public ExposedReference<GameObject> m_gameObj;
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        TestPlayableBehaviour testBehaviour = new TestPlayableBehaviour();
        testBehaviour.m_gameObj = m_gameObj.Resolve(graph.GetResolver());

        var scriptPlayable = ScriptPlayable<TestPlayableBehaviour>.Create(graph, testBehaviour);
        return scriptPlayable;  
    }
}


public class TestPlayableBehaviour : PlayableBehaviour
{
    public GameObject m_gameObj;
    float m_totalTime = 0f;
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
        m_totalTime += info.deltaTime;
        m_gameObj.transform.rotation = Quaternion.AngleAxis(45, new Vector3(0, 45, 0));
    }

}