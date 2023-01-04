using SystemBase.Core;
using SystemBase.Utils;
using UniRx;
using UnityEngine;

namespace Systems.Camera
{
    [GameSystem]
    public class CameraSystem : GameSystem<CameraComponent>
    {
        public override void Register(CameraComponent component)
        {
            SystemUpdate(component)
                .Subscribe(UpdateCamera)
                .AddTo(component);
        }

        private static void UpdateCamera(CameraComponent camera)
        {
            var targetTransform = camera.target.transform;
            var camTransform = camera.transform;
            var targetPosition = targetTransform.position;
            var camPosition = camTransform.position;
            var camToTarget = camPosition.DirectionTo(targetPosition);
            
            var forward = camToTarget * Input.GetAxis("Vertical") * camera.speed;
            var side = new Vector3(camToTarget.z, 0, -camToTarget.x) * Input.GetAxis("Horizontal") * camera.speed;
            var movement = forward + side;
            movement.y = 0;
            
            targetPosition += movement;
            camPosition += movement;
            targetTransform.position = targetPosition;
            camTransform.position = camPosition;
            
            camera.transform.LookAt(camera.target.transform);

            if (!Input.GetMouseButton(0))
            {
                return;
            }
            var x = Input.GetAxis("Mouse X") * 10f;
            camera.transform.RotateAround(targetTransform.position, Vector3.up, x);
        }
    }
}