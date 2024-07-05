using UnityEngine;

public class BombCollider : MonoBehaviour
{
    [SerializeField] EnemyExplosive Enemy = null;
    [SerializeField] GameObject Zone = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            Enemy.playerTakeDamages = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            Enemy.playerTakeDamages = false;
    }

    internal GameObject BombZone()
    {
        return Zone;
    }
}
