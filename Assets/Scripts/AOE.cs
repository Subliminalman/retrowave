using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour {
    public int damage = 10;
    public float radiusOfEffect;
    public float expandRate;
    public float persist = 0.3f;
    private float finishTime = -1;

	void Update () {
        Vector3 scale = transform.localScale;
        float expand = expandRate * Time.deltaTime;    
        if (scale.x + expand <= radiusOfEffect) {
            scale.x += expand;
            scale.y += expand;
            scale.z += expand;
            transform.localScale = scale;
        } else if (finishTime < 0) {
            scale.x = radiusOfEffect;
            scale.y = radiusOfEffect;
            scale.z = radiusOfEffect;
            transform.localScale = scale;
            finishTime = Time.time + persist;
        } else if (Time.time > finishTime) {
            Destroy (gameObject);
        }
	}

    void OnTriggerEnter (Collider col) {
        Player player = col.gameObject.GetComponent<Player> ();
        if (player != null) {
            player.TakeDamage (damage);
        }
    }
}
