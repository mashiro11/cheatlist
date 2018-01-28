using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class forceRotation : MonoBehaviour
{

    Vector3 _lastProfessorPosition = Vector3.zero;
    Dictionary<Going, string> _animations;

    public static forceRotation Instance;
    public Going GoingToward;
    public Animator myAnimator;
    public SpriteRenderer spriteRender;
    public Transform follow;

    public enum Going
    {
        none,
        left,
        right,
        up,
        down
    }

    private void Start()
    {
        Instance = this;
        myAnimator = GetComponent<Animator>();
     

        if (follow == null)
            Debug.LogError("Defina param Follow!");

        _lastProfessorPosition = follow.position;

        _animations = new Dictionary<Going, string>
        {
            { Going.up, "walkUp" },
            { Going.down, "walkDown" },
            { Going.left, "walkLeft" },
            { Going.right, "walkRight" },
            { Going.none, "idle" },
        };
        spriteRender = GetComponent<SpriteRenderer>();
        StartCoroutine(_AnimResolver());

        this.transform.rotation = Quaternion.Euler(270, 0, 180);
    }

    IEnumerator _AnimResolver()
    {
        while (true)
        {
            var difX = Mathf.Abs(Mathf.Abs(follow.position.x) - Mathf.Abs(_lastProfessorPosition.x));
            var difZ = Mathf.Abs(Mathf.Abs(follow.position.z) - Mathf.Abs(_lastProfessorPosition.z));

            if (difX > difZ)
            {
                if (follow.position.x < _lastProfessorPosition.x)
                    GoingToward = Going.left;
                else if (follow.position.x > _lastProfessorPosition.x)
                    GoingToward = Going.right;
            }
            else if (difX < difZ)
            {
                if (follow.position.z < _lastProfessorPosition.z)
                    GoingToward = Going.down;
                else if (follow.position.z > _lastProfessorPosition.z)
                    GoingToward = Going.up;
            }
            else
                GoingToward = Going.none;

            if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName(_animations[GoingToward]))
                myAnimator.Play(_animations[GoingToward]);

            var lNm = spriteRender.sortingLayerName;
            float minhaAltura = follow.position.z;
            var filasMaiores = AlunoSpawner.instance.zPositionAndSortingLayerPerQueue
                .Where(x => x.Value < minhaAltura)
                .OrderBy(x => x.Key);

            if (!filasMaiores.Any())
                spriteRender.sortingLayerName = string.Format("Fileira4");
            else
                spriteRender.sortingLayerName = string.Format("Fileira{0}", filasMaiores.First().Key);

            _lastProfessorPosition = follow.position;
        
            yield return new WaitForSeconds(0.2f);
        }

    }

    void LateUpdate()
    {
        this.transform.position = new Vector3(follow.position.x, follow.position.y, follow.position.z + 1.075f);
    }

    private void FixedUpdate()
    {

    }
}
