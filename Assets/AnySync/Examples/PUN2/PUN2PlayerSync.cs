using Photon.Pun;
using UnityEngine;

namespace AnySync.Examples
{
    /// <summary>
    /// Advanced example ported to Photon Unity Networking 2.
    /// Built on top of serialization instead of using RPCs.
    /// </summary>
    public class PUN2PlayerSync : MonoBehaviourPunCallbacks, IPunObservable
    {
        private const float CharacterSpeed = 3f;
        private const float CharacterRotationSensitivity = 3f;

        private float _angularVelocity;
        private bool _teleportOnNextSync;
        private double _lastReceivedKeyframeTime = double.NegativeInfinity;

        private CharacterController _characterController;
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                // Teleportation.
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    var newPosition = new Vector3(Random.Range(-6f, 6f), 2f, Random.Range(-6f, 6f));
                    // Disabling characterController first so it doesn't override the teleport.
                    _characterController.enabled = false;
                    transform.position = newPosition;
                    _characterController.enabled = true;

                    // In order to teleport, we have to send an additional keyframe with 0 interpolation time.
                    // Since we can't (or don't want to) manually invoke the serialization event, we have to work around it.
                    _teleportOnNextSync = true;
                    return;
                }

                // Local movement.
                var movementInput = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
                _characterController.SimpleMove(new Vector3(-movementInput.x, 0f, -movementInput.y) * CharacterSpeed);
                _angularVelocity = Input.GetAxisRaw("Mouse X") * CharacterRotationSensitivity;
                transform.Rotate(0f, _angularVelocity, 0f);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Sending keyframe data.
                stream.SendNext(transform.position);
                stream.SendNext(_characterController.velocity);
                stream.SendNext(transform.rotation.eulerAngles.y);
                stream.SendNext(_angularVelocity);
                stream.SendNext(_teleportOnNextSync);

                // Reset the teleport trigger.
                _teleportOnNextSync = false;
            }
            else
            {
                // Receiving keyframe data.
                var position = (Vector3)stream.ReceiveNext();
                var velocity = (Vector3)stream.ReceiveNext();
                var rotation = (float)stream.ReceiveNext();
                var angularVelocity = (float)stream.ReceiveNext();
                var isTeleport = (bool)stream.ReceiveNext();

                // Get the message timestamp and use it as a "world time" for this keyframe.
                var keyframeTime = info.SentServerTime;

                // Make the first keyframe instant to avoid issues.
                if (_lastReceivedKeyframeTime < 0)
                    _lastReceivedKeyframeTime = keyframeTime;

                // Get time difference from last keyframe and feed it as interpolation time.
                var interpolationTime = (float)(keyframeTime - _lastReceivedKeyframeTime);
                _lastReceivedKeyframeTime = keyframeTime;           
            }
        }
    }
}
