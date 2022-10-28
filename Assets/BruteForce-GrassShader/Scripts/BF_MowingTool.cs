using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BF_MowingTool : MonoBehaviour
{
    public float distanceToFillMarkers = 0.9f;
    public ParticleSystem particleFeedback;

    private BF_MowingManager mM;
    private ParticleSystem pS;
    private ParticleSystem.Particle[] particles;
    private int numParticlesAlive;
    private bool isWaiting = false;
    private float closestMarker = 10000f;

    void Start()
    {
        pS = this.GetComponent<ParticleSystem>();
        if (particleFeedback != null)
        {
            particleFeedback.Play();
        }
    }

    void Update()
    {
        int layerMask = 1 << 0;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, 20, layerMask))
        {
            BF_MowingManager tempMM = hit.transform.GetComponent<BF_MowingManager>();
            if (tempMM != null)
            {
                if (tempMM != mM)
                {
                    mM = tempMM;
                }
            }
            else
            {
                mM = null;
            }
        }

        if (mM == null)
        {
            pS.Stop();
            if (particleFeedback != null)
            {
                particleFeedback.Stop();
            }
            return;
        }

        particles = new ParticleSystem.Particle[pS.particleCount];
        numParticlesAlive = pS.GetParticles(particles);

        if (numParticlesAlive > 0)
        {
            foreach (ParticleSystem.Particle particle in particles)
            {
                if (Vector3.Distance(pS.transform.position, particle.position) < 0.4f)
                {
                    if (particle.remainingLifetime < pS.main.startLifetime.constant - 0.4f)
                    {
                        if (pS.isEmitting)
                        {
                            pS.Stop();
                            if (particleFeedback != null)
                            {
                                particleFeedback.Stop();
                            }
                        }
                        return;
                    }
                }
            }
            if (!pS.isEmitting)
            {
                pS.Play();
                if (particleFeedback != null)
                {
                    particleFeedback.Play();
                }
            }

            Vector3 particlePos = pS.transform.position;
            List<Vector3> tempList = new List<Vector3>();
            closestMarker = 1000f;
            foreach (Vector3 pos in mM.markersPos)
            {
                // Distance between the particle emitted and closest marker
                float distanceToMarker = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(particlePos.x, 0, particlePos.z));
                if (distanceToMarker < closestMarker)
                {
                    closestMarker = distanceToMarker;
                }

                if (distanceToMarker < distanceToFillMarkers)
                {
                    tempList.Add(pos);
                }
            }


            if (closestMarker > distanceToFillMarkers*2f)
            {

                if (!isWaiting && particleFeedback != null)
                {
                    isWaiting = true;
                    StartCoroutine(WaitForFeedback());
                }
            }
            else if (!isWaiting && !particleFeedback.isEmitting && particleFeedback != null)
            {
                particleFeedback.Play();
            }

            foreach (Vector3 posToRemove in tempList)
            {
                mM.markersPos.Remove(posToRemove);
            }

        }
    }

    private IEnumerator WaitForFeedback()
    {
        yield return new WaitForSeconds(0.2f);
        if(closestMarker > distanceToFillMarkers*2f)
        {
            particleFeedback.Stop();
        }
        else if(!particleFeedback.isEmitting)
        {
            particleFeedback.Play();
        }

        isWaiting = false;
    }
}
