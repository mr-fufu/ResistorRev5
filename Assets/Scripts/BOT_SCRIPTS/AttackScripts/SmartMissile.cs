using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissile : MonoBehaviour
{
    public float start_velocity;
    public float attenuation;
    public float acceleration;
    public float y_range;
    public float anim_range;

    public float start_acceleration;
    public float attenuated_range;
    public float velocity;

    public int direction = 1;
    public float datum;
    public bool anim_lock;

    public Animator anim;
    public float slingshot;
    public float decay;

    // Start is called before the first frame update
    void Start()
    {
        start_acceleration = acceleration;
        datum = GetComponent<Projectile>().ground_y_position + 30f;
        velocity = start_velocity;
        anim = gameObject.GetComponent<Animator>();
        attenuated_range = y_range;
        decay = 1;
    }

    // Update is called once per frame
    void Update()
    {
        decay += 0.00001f;
        velocity += acceleration * direction * 0.01f;
        velocity += slingshot * direction * 0.01f;
        velocity = velocity / decay;

        // Actual transform modification
        transform.Translate(new Vector3(0f, velocity , 0f));

        // if the missile's vertical velocity slows below a certain threshold, the acceleration becomes 0 and the missile stops moving up/down
        if (acceleration <= 0.1f * start_acceleration)
        {
            anim.SetBool("MoveUp", false);
            anim.SetBool("MoveDown", false);
        }
        else
        {
            // if the missile is in the animation range, set the appropriate animation state (i.e. pointing up/down)
            // added the anim_lock so that the missile will animate a change in direction at the peak of the ascent
            if (!anim_lock)
            {
                if (anim_range > 0.1f)
                {
                    if ((transform.position.y < (datum + anim_range)) && (transform.position.y > (datum - anim_range)))
                    {
                        if (direction == 1)
                        {
                            anim.SetBool("MoveUp", true);
                            anim.SetBool("MoveDown", false);
                        }
                        else if (direction == -1)
                        {
                            anim.SetBool("MoveUp", false);
                            anim.SetBool("MoveDown", true);
                        }
                    }
                    else
                    {
                        anim.SetBool("MoveUp", false);
                        anim.SetBool("MoveDown", false);
                    }
                }
                else
                {
                    anim.SetBool("MoveUp", false);
                    anim.SetBool("MoveDown", false);
                }

            }

            // if the anim_lock is on, check for when the missile passes the datum (within a certain range) and disable the anim_lock
            // then increase the attenuation of the acceleration and the animated and movement ranges
            if (anim_lock)
            {
                if ((transform.position.y < (datum + 0.1f)) && (transform.position.y > (datum - 0.1f)))
                {
                    anim_lock = false;

                    acceleration = attenuation * acceleration;
                    attenuated_range = (attenuation * attenuated_range);
                    anim_range = (attenuation * anim_range);
                    decay = decay * decay;
                }
            }

            // check if the missile has reached the maximum vertical range and reverse the direction
            if (transform.position.y > (datum + attenuated_range))
            {
                direction = -1;
                anim_lock = true;

                anim.SetBool("MoveUp", false);
                anim.SetBool("MoveDown", true);
            }
            else if (transform.position.y < (datum - attenuated_range))
            {
                direction = 1;
                anim_lock = true;

                anim.SetBool("MoveUp", true);
                anim.SetBool("MoveDown", false);
            }

            slingshot = transform.position.y - datum;

            if (transform.position.y > datum)
            {
                slingshot = slingshot * 0.05f;
            }
            else
            {
                slingshot = slingshot * -0.05f;
            }

        }
    }
}
