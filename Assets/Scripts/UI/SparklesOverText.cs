using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparklesOverText : MonoBehaviour
{

    [Header("Sparkles over Text Settings: ")]
    [SerializeField] private GameObject sparkle;
    [SerializeField] [Range(0, 5)] private float minWaitSpawnTime;
    [SerializeField] [Range(0, 5)] private float maxWaitSpawnTime;
    [SerializeField][Range(0, 5)] private float sparkleLifeTime;
    [SerializeField] private RectTransform textBounds;

    // Start is called before the first frame update
    private void Start() => StartCoroutine(SpawnSparkles());

    private IEnumerator SpawnSparkles()
    {
        while(true)
        {
            var waitTime = Random.Range(minWaitSpawnTime, maxWaitSpawnTime);
            yield return new WaitForSeconds(waitTime);

            var randPos = new Vector2(Random.Range(textBounds.rect.xMin, textBounds.rect.xMax), Random.Range(textBounds.rect.yMin, textBounds.rect.yMax));
            var worldPos = textBounds.TransformPoint(randPos);

            var newSparkle = Instantiate(sparkle, worldPos, Quaternion.identity, textBounds);
            Destroy(newSparkle, sparkleLifeTime);
        }
    }
}
