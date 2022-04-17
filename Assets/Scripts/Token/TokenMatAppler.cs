using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace App
{
    public class TokenMatAppler : MonoBehaviour
    {
        [Serializable]
        public class TeamMats
        {
            public Material RedMat;
            public Material BlueMat;

            public Material this[Team team]
            {
                get
                {
                    switch (team)
                    {
                        case Team.Red:
                            return RedMat;
                        case Team.Blue:
                            return BlueMat;
                    }
                    return null;
                }
            }
        }
        public TeamMats mats;

        public GameObject[] teamObjects;

        private void Start()
        {
            SetTeamMat();
        }

        public void SetTeamMat()
        {
            var token = this.GetComponent<TokenBase>();

            if (token != null && token.team != Team.None)
            {
                foreach (var obj in teamObjects)
                {
                    var render = obj.GetComponent<MeshRenderer>();
                    if (render != null)
                    {
                        render.material = mats[token.team];
                    }
                }
            }
        }
    }
}