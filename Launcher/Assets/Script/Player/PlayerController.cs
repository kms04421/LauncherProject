using UnityEngine;
using UnityEngine.InputSystem;
enum CharacterState
{
    DefaultState,
    OperationState,
    DieState,
}
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterState currentState;

    private CharacterController characterController;

    [SerializeField] private float moveSpeed = 4f;

    private float gravity = -9.81f;

    private Vector3 _direction;

    private Vector2 _input;

    private float rotationSpeed = 500f;

    [SerializeField] private float gravityMultiplier = 3.0f;

    [SerializeField] private float jumpPower;

    private float _velocity;

    private bool _isjump = false;

    private Camera _mainCamera;

    private Quaternion playerQuaternion;

    private Animator animator;

    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 10f;
    public float reach = 8f;
    World world;
    PlayerUI playerUI;
    Item item;
    private byte selectNumber = 0;

    void Start()
    {
        item = new Item();
        playerUI = GetComponent<PlayerUI>();
        world = GameObject.Find("World").GetComponent<World>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        TransitionToState(CharacterState.DefaultState);
        _mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CharacterState.DefaultState:
                UpdateDefaultState();
                break;
            case CharacterState.OperationState:
                UpdateOperationState();
                break;
        }
    }


    private void TransitionToState(CharacterState newState)
    {
        // 이전 상태에서 나가는 동작 수행
        switch (currentState)
        {
            case CharacterState.DefaultState:

                break;
            case CharacterState.OperationState:

                break;

        }




        switch (newState)
        {
            case CharacterState.DefaultState:

                break;
            case CharacterState.OperationState:

                break;
        }


        // 새로운 상태로 변경
        currentState = newState;

    }
    private void UpdateDefaultState()
    {
        ApplayRotation();
        ApplyGravity();
        ApplyMovement();


    }

    private void UpdateOperationState()
    {

    }

    private void placeCurosrBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = _mainCamera.transform.position + (_mainCamera.transform.forward * step);

            if (world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }



    public void ApplayRotation()
    {

        _direction = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(_input.x, 0.0f, _input.y);
        playerQuaternion = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, playerQuaternion, rotationSpeed * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if (IsGrounded() && _isjump == false)
        {
            _velocity = -1.0f;
        }
        else
        {
            if (_isjump)
            {
                IsJump();
            }
            _velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    public void ApplyMovement()
    {
        if (_input.sqrMagnitude != 0)
        {
            animator.SetInteger("AnimationPar", 1);
        }
        else
        {
            animator.SetInteger("AnimationPar", 0);
        }
        characterController.Move(_direction * moveSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0.0f, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (!context.started) return;
        if (!IsGrounded()) return;

        _velocity += jumpPower;
        IsJump();


    }

    public void LClick(InputAction.CallbackContext context)
    {
        placeCurosrBlocks();
        if (!context.started) return;
        if (!highlightBlock.gameObject.activeSelf) return;
        item.ItemByteconversion(world.GetChunkFromVector3(highlightBlock.position).GetTOVoxel(highlightBlock.position));
        playerUI.inventory.AddItem(item);
      
        world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);


    }
    public void RClick(InputAction.CallbackContext context)
    {
        placeCurosrBlocks();
        if (!context.started) return;
        if (!placeBlock.gameObject.activeSelf) return;
       
        
        world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, playerUI.itemSlot[selectNumber].itemCode);

    }

    public void NumberInput(InputAction.CallbackContext context)
    {
        playerUI.NumberInput((int.Parse(context.control.name) - 1));
        selectNumber = (byte)(int.Parse(context.control.name)-1);

    }

    private bool IsJump() => _isjump = !_isjump;
    private bool IsGrounded() => characterController.isGrounded;
}
