using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    [Header("UI References")]
    public Button rollButton;
    public Text resultText;
    public Text pointsText;
    public Text instructionText;

    [Header("Settings")]
    public float rollForce = 5f;
    public float torqueForce = 10f;

    private GameObject dice;
    private Rigidbody diceRb;
    private bool isRolling = false;
    private bool hasRolled = false;

    void Start()
    {
        CreateGround();
        CreateWalls();
        CreateDice();

        rollButton.onClick.AddListener(OnRollPressed);
        resultText.text = "";
        pointsText.text = "";
        instructionText.text = "Roll the dice to determine\nyour starting points!";
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0, -0.5f, 0);
        ground.transform.localScale = new Vector3(8, 1, 8);

        Renderer rend = ground.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.24f, 0.17f, 0.12f);
        rend.material = mat;

        PhysicsMaterial groundPhysMat = new PhysicsMaterial("GroundMat");
        groundPhysMat.bounciness = 0.3f;
        groundPhysMat.dynamicFriction = 0.6f;
        groundPhysMat.staticFriction = 0.6f;
        ground.GetComponent<Collider>().material = groundPhysMat;
    }

    void CreateWalls()
    {
        float wallHeight = 2f;
        float wallThickness = 0.5f;
        float arenaSize = 4f;

        Vector3[] positions = {
            new Vector3(0, wallHeight / 2, arenaSize),
            new Vector3(0, wallHeight / 2, -arenaSize),
            new Vector3(arenaSize, wallHeight / 2, 0),
            new Vector3(-arenaSize, wallHeight / 2, 0)
        };

        Vector3[] scales = {
            new Vector3(arenaSize * 2, wallHeight, wallThickness),
            new Vector3(arenaSize * 2, wallHeight, wallThickness),
            new Vector3(wallThickness, wallHeight, arenaSize * 2),
            new Vector3(wallThickness, wallHeight, arenaSize * 2)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall_" + i;
            wall.transform.position = positions[i];
            wall.transform.localScale = scales[i];
            Renderer rend = wall.GetComponent<Renderer>();
            rend.enabled = false;
        }
    }

    void CreateDice()
    {
        dice = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dice.name = "Dice";
        dice.transform.position = new Vector3(0, 3f, 0);
        dice.transform.localScale = Vector3.one * 1f;

        Renderer rend = dice.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = Color.white;
        rend.material = mat;

        diceRb = dice.AddComponent<Rigidbody>();
        diceRb.mass = 0.1f;
        diceRb.linearDamping = 0.5f;
        diceRb.angularDamping = 0.5f;
        diceRb.useGravity = true;
        diceRb.isKinematic = true;

        PhysicsMaterial diceMat = new PhysicsMaterial("DiceMat");
        diceMat.bounciness = 0.4f;
        diceMat.dynamicFriction = 0.4f;
        diceMat.staticFriction = 0.4f;
        dice.GetComponent<Collider>().material = diceMat;

        CreateAllFaceDots();
    }

    void CreateAllFaceDots()
    {
        float offset = 0.51f;
        float dotSize = 0.12f;
        float spread = 0.28f;

        Material dotMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        dotMat.color = Color.black;

        // Face 1 (+Y) - 1 dot
        CreateDot(Vector3.up * offset, dotSize, dotMat);

        // Face 6 (-Y) - 6 dots
        CreateDot(new Vector3(-spread, -offset, spread), dotSize, dotMat);
        CreateDot(new Vector3(-spread, -offset, 0), dotSize, dotMat);
        CreateDot(new Vector3(-spread, -offset, -spread), dotSize, dotMat);
        CreateDot(new Vector3(spread, -offset, spread), dotSize, dotMat);
        CreateDot(new Vector3(spread, -offset, 0), dotSize, dotMat);
        CreateDot(new Vector3(spread, -offset, -spread), dotSize, dotMat);

        // Face 2 (+Z) - 2 dots
        CreateDot(new Vector3(-spread, spread, offset), dotSize, dotMat);
        CreateDot(new Vector3(spread, -spread, offset), dotSize, dotMat);

        // Face 5 (-Z) - 5 dots
        CreateDot(new Vector3(-spread, spread, -offset), dotSize, dotMat);
        CreateDot(new Vector3(spread, spread, -offset), dotSize, dotMat);
        CreateDot(new Vector3(0, 0, -offset), dotSize, dotMat);
        CreateDot(new Vector3(-spread, -spread, -offset), dotSize, dotMat);
        CreateDot(new Vector3(spread, -spread, -offset), dotSize, dotMat);

        // Face 3 (+X) - 3 dots
        CreateDot(new Vector3(offset, spread, -spread), dotSize, dotMat);
        CreateDot(new Vector3(offset, 0, 0), dotSize, dotMat);
        CreateDot(new Vector3(offset, -spread, spread), dotSize, dotMat);

        // Face 4 (-X) - 4 dots
        CreateDot(new Vector3(-offset, spread, spread), dotSize, dotMat);
        CreateDot(new Vector3(-offset, spread, -spread), dotSize, dotMat);
        CreateDot(new Vector3(-offset, -spread, spread), dotSize, dotMat);
        CreateDot(new Vector3(-offset, -spread, -spread), dotSize, dotMat);
    }

    void CreateDot(Vector3 localPos, float size, Material mat)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.name = "Dot";
        dot.transform.SetParent(dice.transform, false);
        dot.transform.localPosition = localPos;
        dot.transform.localScale = Vector3.one * size;
        dot.GetComponent<Renderer>().material = mat;
        Destroy(dot.GetComponent<Collider>());
    }

    void OnRollPressed()
    {
        if (isRolling || hasRolled) return;

        isRolling = true;
        rollButton.gameObject.SetActive(false);
        instructionText.text = "";
        resultText.text = "Rolling...";

        dice.transform.position = new Vector3(
            Random.Range(-0.5f, 0.5f),
            3f,
            Random.Range(-0.5f, 0.5f)
        );

        dice.transform.rotation = Random.rotation;

        diceRb.isKinematic = false;
        diceRb.linearVelocity = Vector3.zero;
        diceRb.angularVelocity = Vector3.zero;

        Vector3 force = new Vector3(
            Random.Range(-2f, 2f),
            -rollForce,
            Random.Range(-2f, 2f)
        );
        diceRb.AddForce(force, ForceMode.Impulse);

        Vector3 torque = new Vector3(
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce)
        );
        diceRb.AddTorque(torque, ForceMode.Impulse);

        StartCoroutine(WaitForDiceStop());
    }

    IEnumerator WaitForDiceStop()
    {
        yield return new WaitForSeconds(0.5f);

        float stillTime = 0f;
        float requiredStillTime = 1.0f;
        float startTime = Time.time;
        float timeoutDuration = 10f;

        while (stillTime < requiredStillTime)
        {
            if (diceRb.linearVelocity.magnitude < 0.05f && diceRb.angularVelocity.magnitude < 0.05f)
            {
                stillTime += Time.deltaTime;
            }
            else
            {
                stillTime = 0f;
            }
            yield return null;

            if (Time.time - startTime > timeoutDuration)
            {
                break;
            }
        }

        diceRb.isKinematic = true;

        int result = GetTopFace();
        int startingPoints = result * 100;

        resultText.text = "You rolled a " + result + "!";
        pointsText.text = "Starting with " + startingPoints + " points!";

        GameData.StartingPoints = startingPoints;
        hasRolled = true;
        rollButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene("SlotGame");
    }

    int GetTopFace()
    {
        Vector3[] faceDirections = {
            dice.transform.up,
            -dice.transform.up,
            dice.transform.forward,
            -dice.transform.forward,
            dice.transform.right,
            -dice.transform.right
        };

        int[] faceValues = { 1, 6, 2, 5, 3, 4 };

        float maxDot = -1f;
        int topFace = 1;

        for (int i = 0; i < faceDirections.Length; i++)
        {
            float dot = Vector3.Dot(faceDirections[i], Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                topFace = faceValues[i];
            }
        }

        return topFace;
    }
}
