using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoMove : NetworkBehaviour {

    public Collider2D engaged_target;

    public float move_dist;
    public float animated_move_speed;
    public float animated_move_dist;

    [SyncVar] public bool engaged_check = false;
    public bool walker_check;
    [SyncVar] public bool spawned;

    [SyncVar] public bool animate_check;

    [SyncVar] public bool enemy_check;
    private int move_dir;
    private bool engaged_scan;

    // Important!!!! Scan size determines how many colliders will be detected colliding with the part
    // set this too low and game will result in clipping issues. Approximate for upper limit of
    // bots + projectiles that can collide with the bot at one time.
    private int scan_size = 80;

    Collider2D[] check_colliders = new Collider2D[1];
    ContactFilter2D collider_filter = new ContactFilter2D();


    // Use this for initialization
    void Start () {

        // get move_dist from the speed of the bot, this is the main
        // variable that controls how fast the robot moves
        move_dist = gameObject.GetComponent<StandardStatBlock>().SPEED;

        // move speed is set equal to two times the move_dist to be used if the bot has an
        // attached AutoMoveAnimate script attached which plays the movement animation
        // (if the leg part is animated i.e. walkers)
        animated_move_speed = move_dist * 2;

        // Set the collider filter to no filter (can be changed
        // later to filter the layer the robot is on i.e. the lane)
        collider_filter = collider_filter.NoFilter();

        //collider_filter.layerMask = gameObject.layer;

        // checks the standard stat block to see whether the robot belongs
        // to the player or is an enemy robot (ENEMY is false if belongs to player
        // on the left or true if player is on the right)
        enemy_check = gameObject.GetComponent<StandardStatBlock>().ENEMY;

    }

    // Update is called once per frame
    void Update () {

        if (spawned)
        {
            //-------------------------------------------------------------------
            // Set Walk Direction based on Enemy or Player owned bot (enemy_check bool)

            if (!enemy_check)
            {
                move_dir = 1;
                gameObject.tag = "BOT_Player";
            }
            else if (enemy_check)
            {
                move_dir = 1;
                gameObject.tag = "BOT_Enemy";
            }

            //-------------------------------------------------------------------
            // Set Bot to walk if not Engaged

            if (walker_check)
            {
                if (!engaged_check)
                {
                    animate_check = true;
                }
                else if (engaged_check)
                {
                    animate_check = false;
                }
            }

            // Otherwise, if the robot is not a walker, translate the robot
            // and perform animations if the AutoMoveAnimate script is also attached
            // to the same gameObject

            else if (!walker_check)
            {
                if (!engaged_check)
                {
                    animate_check = true;
                    gameObject.transform.Translate(move_dir*move_dist*0.25f*Time.deltaTime*50, 0, 0);
                }
                else if (engaged_check)
                {
                    animate_check = false;
                }
            }

            //-------------------------------------------------------------------
            // Disengage if the target is destroyed (receive destruction trigger 
            // prior to destruction from plating script of engaged target)
            // Also Resets engaged target if it has been destroyed

            // Commented out for multiplayer (behaving weird)

            //if (engaged_target != null && engaged_check == true)
            //{
            //    if (engaged_target.gameObject.GetComponent<Plating>().destruction_trigger == true)
            //    {
            //        //engaged_target = gameObject.GetComponent<Collider2D>();
            //        engaged_target = null;
            //        engaged_check = false;
            //    }
            //}

            //-------------------------------------------------------------------
            // checks if the robot is currently collided with the robot in front of it
            // i.e. engaged_target. engaged_target is always set to the robot in front of the
            // gameObject on the same lane
            // if the gameObject is not currently collided with engaged_target
            // then nothing is blocking it and it is therefore set disengaed

            // reset check_colliders to an empty array

            check_colliders = new Collider2D[scan_size];

            for (int setVar = 0; setVar < scan_size; setVar++)
            {
                check_colliders[setVar] = null;
            }

            // scan for all overlapping 2d colliders

            gameObject.GetComponent<Collider2D>().OverlapCollider(collider_filter, check_colliders);

            // check through all elements of check_colliders
            // to see if any of them is the engaged_target

            for (int checkVar = 0; checkVar < scan_size; checkVar++)
            {
                if ((engaged_target != null) && (check_colliders[checkVar] == engaged_target))
                {
                    engaged_check = true;
                    engaged_scan = true;
                }
            }

            // if none of the elements are the engaged_target then
            // set disengaged (i.e. engaged_check is false)

            if (!engaged_scan)
            {
                engaged_check = false;
            }

            engaged_scan = false;
        }
    }

    private void OnTriggerStay2D(Collider2D EnterCollider)
    {
        // upon entering a collider, check whether that collider is 
        // a bot(player OR enemy owned) and whether if that robot is in front of the gameObject robot
        // (engaged_target to the right if gameObject is a player bot, left if enemy)
        // if true, then the robot is engaged in combat/unable to move forward
        // engaged robot is set to the engaged_target

        // also, replaces engaged_target with a new engaged_target if new engaged_target
        // is closer to the gameObject robot

        Vector3 OtherCenter = EnterCollider.gameObject.transform.position;
        Vector3 ObjectCenter = gameObject.transform.position;

        if ((engaged_target != null && Mathf.Abs(engaged_target.transform.position.x - ObjectCenter.x) > Mathf.Abs(OtherCenter.x - ObjectCenter.x)) || engaged_target == null)
        {
            if ((enemy_check) && (OtherCenter.x < ObjectCenter.x) && (EnterCollider.gameObject.tag == "BOT_Enemy" || EnterCollider.gameObject.tag == "BOT_Player"))
            {
                engaged_target = EnterCollider;
                engaged_check = true;
                animate_check = false;
            }
            else if (!enemy_check && OtherCenter.x > ObjectCenter.x && (EnterCollider.gameObject.tag == "BOT_Player" || EnterCollider.gameObject.tag == "BOT_Enemy"))
            {
                engaged_target = EnterCollider;
                engaged_check = true;
                animate_check = false;
            }
        }
    }

    //-------------------------------------------------------------------------
    // Called during WalkState animation in order to move forward

    public void SkipMove()
    {
        if (isServer)
        {
            transform.Translate(new Vector3(animated_move_dist * move_dir * 8, 0f, 0f));
        }
    }
}

// Time.deltaTime