using DG.Tweening;
using UnityEngine;

public class DraggableObject : MonoBehaviour {
    [SerializeField] private float _distanceRaycast = 5f, _tweenSpeed = 1.5f;
    [SerializeField] private LayerMask _layerToDetect;
    [SerializeField] private Vector3 _targetPosition, _targetRotation;
    [SerializeField] private Material _redPlatform;
    [SerializeField] private Material _greenPlatform;
    [SerializeField] private GameObject _platform;
    private bool _isDraggable;
    private const string _platformTag = "Indicator";
    private Outline _outline;
    private Rigidbody _rigidbody;

    private void Start() {
        _outline = GetComponent<Outline>();
        _rigidbody = GetComponent<Rigidbody>();
        _outline.enabled = false;

        _isDraggable = IsOutside();
        _rigidbody.isKinematic = !_isDraggable;

        if (!_isDraggable) {
            LockObject();
        }
    }

    private void OnMouseEnter() {
        if (_isDraggable) _outline.enabled = true;
    }

    private void OnMouseExit() {
        _outline.enabled = false;
    }

    private void OnMouseDown() {
        Vector3 mousePointer = Input.mousePosition;
        mousePointer.z = Camera.main.WorldToScreenPoint(transform.position).z;
        _rigidbody.isKinematic = true;
    }

    private void OnMouseDrag() {
        if (!_isDraggable) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Vector3 newPosition = hit.point;
            newPosition.y = this.transform.position.y;
            this.transform.position = newPosition;
        }

        RaycastHit _hit;
        bool _objectRaycast = Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out _hit, _distanceRaycast, _layerToDetect);
        if (_objectRaycast && _hit.collider.gameObject.CompareTag(_platformTag)) {
            ChangePlatformMaterial(_platform, _greenPlatform);
        }
        else {
            ResetPlatformMaterial(_platform);
        }
    }

    private void OnMouseUp() {
        if (!IsOutside()) {
            _isDraggable = false;
            LockObject();
        }
        else {
            _rigidbody.isKinematic = false;
        }
        ResetPlatformMaterial(_platform);
    }

    private bool IsOutside() {
        return !Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, _distanceRaycast, _layerToDetect) ||
               !hit.collider.CompareTag(_platformTag);
    }

    private void LockObject() {
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        transform.DOMove(_targetPosition, _tweenSpeed)
                 .SetEase(Ease.OutQuad);

        transform.DORotate(_targetRotation, _tweenSpeed)
                 .SetEase(Ease.OutQuad);
    }

    private void ChangePlatformMaterial(GameObject platform, Material material) {
        Renderer _renderer = platform.GetComponent<Renderer>();
        _renderer.material = material;
    }

    private void ResetPlatformMaterial(GameObject platform) {
        Renderer _renderer = platform.GetComponent<Renderer>();
        _renderer.material = _redPlatform;
    }
}
