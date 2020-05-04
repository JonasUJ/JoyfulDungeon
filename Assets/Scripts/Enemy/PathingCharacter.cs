using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathingAI))]
public class PathingCharacter : Character
{
    public GameObject Target;
    public float MoveSpeed = 1f;
    PathingAI _pathing;
    PathingAI pathing
    {
        get
        {
            if (_pathing == null)
                _pathing = GetComponent<PathingAI>();
            return _pathing;
        }
    }
    bool finished = true;
    Vector3 moveVel = Vector3.zero;
    float lastTraverse = 0f;
    float maxPathTime = 1f;
    bool renew = false;

    protected void UpdatePathing()
    {
        if (Time.time > lastTraverse + maxPathTime)
        {
            ForcePathingNextUpdate();
            lastTraverse = Time.time;
        }
        if (controller.isGrounded)
            moveVel.y = 0;
        moveVel.y += NavMeshGenerator.Gravity * Time.deltaTime;
        if (finished)
        {
            pathing.FindPath(transform.position, Target.transform.position, renew);
            if (pathing.DestinationReached)
                moveVel.x *= 0.95f;
            else
                TraverseEdge(pathing.NextEdge());
            lastTraverse = Time.time;
        }
        moveVel += Knockback *= 0.7f;
        controller.move(moveVel * Time.deltaTime);
    }

    protected void ForcePathingNextUpdate()
    {
        renew = true;
    }

    void TraverseEdge(Edge edge)
    {
        finished = false;
        switch (edge.type)
        {
            case LinkType.Straight:
                StartCoroutine(TraverseStraight(edge));
                break;
            case LinkType.Fall:
                StartCoroutine(TraverseFall(edge));
                break;
            case LinkType.Jump:
                moveVel = Vector3.zero;
                StartCoroutine(TraverseJump(edge));
                break;
        }
    }

    IEnumerator TraverseStraight(Edge edge)
    {
        float direction = Mathf.Sign(edge.end.pos.x - edge.start.pos.x);
        renew = false;

        while ((transform.position.x - (edge.end.pos + NavMeshGenerator.Offset).x) * direction <= 0f && !renew)
        {
            moveVel.x = Mathf.Lerp(moveVel.x, direction * MoveSpeed, Time.deltaTime * MoveSpeed);
            yield return null;
        }
        finished = true;
    }

    IEnumerator TraverseFall(Edge edge)
    {
        float time = Mathf.Sqrt(2 * (edge.end.pos.y - edge.start.pos.y) / NavMeshGenerator.Gravity);
        float direction = Mathf.Sign(edge.end.pos.x - edge.start.pos.x);
        renew = false;

        while ((transform.position.x - (edge.end.pos + NavMeshGenerator.Offset).x) * direction <= 0 && !renew)
        {
            moveVel.x = Mathf.Lerp(moveVel.x, direction * MoveSpeed, Time.deltaTime * MoveSpeed);
            yield return null;
        }
        moveVel = Vector3.zero;
        yield return new WaitForSeconds(time);
        finished = true;
    }

    IEnumerator TraverseJump(Edge edge)
    {
        renew = false;
        moveVel = edge.jump.velocity;
        // transform.position = edge.start.pos + NavMeshGenerator.Offset;
        yield return new WaitForSeconds(edge.jump.time);
        finished = true;
    }
}
