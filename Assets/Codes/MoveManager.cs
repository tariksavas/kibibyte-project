using UnityEngine;
using UnityEngine.UI;
public class MoveManager : MonoBehaviour
{
    [SerializeField] private Text scoreText = null;
    [SerializeField] private GameObject stoppedMenu = null;
    [SerializeField] private GameObject finishMenu = null;
    [SerializeField] private GameObject steps = null;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveStepsSpeed = 5;
    private Animator beltAnimator = null;
    private Vector3 bottlesTarget;
    private Vector3 stepsTarget;
    private int bottlesIndex = 0;
    private int refBottlesLength = 0;
    private float bottleHorizontalSize = 0;
    private float horizontal = 0;
    private bool move;
    private bool moveSteps;
    public GameObject[] bottles = null;
    public int completedBottleCount = 0;
    public int stepNum = 0;
    public int colorConfirmedCount = 0;
    public int pillConfirmedCount = 0;
    public int bottleConfirmedCount = 0;
    public int score = 0;
    public static MoveManager moveManagerClass;

    //Bu script bottleParent objesinde bulumaktadır. Şişelerin ve step objectlerin hareketini sağlamaktadır.
    private void Start()
    {
        moveManagerClass = this;

        stepNum = 1;
        move = false;
        moveSteps = false;

        //CreateManager scriptindeki ekran ölçüleri için bulunan değerler ve animator componenti bu scriptteki değişkenlere atanır.
        beltAnimator = CreateManager.createManagerClass.beltAnimator;
        refBottlesLength = CreateManager.createManagerClass.refBottlesLength;
        horizontal = CreateManager.createManagerClass.horizontal;
        bottleHorizontalSize = CreateManager.createManagerClass.bottleHorizontalSize;
    }
    private void Update()
    {
        if (scoreText.text != score.ToString())
        {
            //Score değerinde bir değişiklik olursa Canvas üzerindeki Text'e yazılır.
            scoreText.text = score.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Oyunu duraklatma
            Time.timeScale = 0;
            stoppedMenu.SetActive(true);
        }
        if (completedBottleCount >= bottles.Length)
        {
            //Her stepte, her bir şişenin görevi yapıldıktan sonra completedBottleCount değeri 1 arttırılır.
            MoveSteps();
        }
    }
    private void FixedUpdate()
    {
        if (move)
        {
            //Editörden alınan bant hızına göre, hareket metodunda hesaplanan bottlesTarget değerine hareket edilir.
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, bottlesTarget, step);
            if(transform.position.x == bottlesTarget.x)
            {
                //Hedef pozisyona varıldığında banda ait hareket animasyonu durur.
                move = false;
                beltAnimator.SetBool("move", move);
            }
        }
        if (moveSteps)
        {
            //Editörden alınan steps hızına göre, stepsTarget değerine hareket edilir.
            float step = moveStepsSpeed * Time.deltaTime;
            steps.transform.position = Vector3.MoveTowards(steps.transform.position, stepsTarget, step);
            if (steps.transform.position.x == stepsTarget.x)
                moveSteps = false;
        }
    }
    public void UpdateTargetPos()
    {
        if(bottlesIndex < bottles.Length)
        {
            if(stepNum == 2)
                CreateManager.createManagerClass.translateCompleted = false;
            //İlgili stepteki tüm şişelerin görevleri tamamlanmadıysa bu işlemler yapılır.
            for (int i = 0; i < refBottlesLength; i++)
            {
                if (stepNum == 2)
                {
                    //2. stepte şişeler kapakları açık vaziyette gelir.
                    bottles[bottlesIndex].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                }
                else if(stepNum == 3)
                {
                    //3. stepte şişeler selectable edilir ve şişelerin içerisindeki hapların Rigidbody ve Collider componenti silinir.
                    bottles[bottlesIndex].GetComponent<BottleManager>().selectable = true;
                    for (int j = 0; j < bottles[bottlesIndex].transform.childCount; j++)
                    {
                        //Haplar 9. layer'a aittir.
                        if (bottles[bottlesIndex].transform.GetChild(j).gameObject.layer == 9)
                            Destroy(bottles[bottlesIndex].transform.GetChild(j).GetComponent<Rigidbody>());
                            Destroy(bottles[bottlesIndex].transform.GetChild(j).GetComponent<CapsuleCollider>());
                    }
                }
                bottles[bottlesIndex].transform.position = new Vector3((i * bottleHorizontalSize) - horizontal + (horizontal / refBottlesLength) - 2, 0, -8);
                
                //Eğer şişe inaktif ise aktif edilir.
                if(!bottles[bottlesIndex].activeSelf)
                    bottles[bottlesIndex].SetActive(true);

                bottlesIndex++;
            }
            move = true;
        }
        else
        {
            //İlgili stepteki tüm işlemler tamamlandıktan sonra bant daha hızlı hareket eder ve sonraki step'e geçilir.
            moveSpeed *= 2;
            move = true;

            if (stepNum == 3)
                move = false;
        }
        //Şişeler x ekseninde 2 birim sağı hedef alır ve bant animasyonu eğer ki "move" değişkeni true ise oynatılır.
        bottlesTarget = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
        beltAnimator.SetBool("move", move);
    }
    private void MoveSteps()
    {
        //Şişelerin tamamının görevi yapıldıktan sonra sonraki aşamaya geçilir.
        if(stepNum < 3)
        {
            moveSpeed /= 2;

            //İki step arası mesafe x ekseninde 5 birim olması sebebiyle stepsTarget değişkenine ilgili pozisyon değeri atanır.
            stepsTarget = new Vector3(steps.transform.position.x - 5, steps.transform.position.y, steps.transform.position.z);
            moveSteps = true;
            bottlesIndex = 0;
            completedBottleCount = 0;
            stepNum++;
            UpdateTargetPos();
        }
        else
        {
            //Son stepte tüm şişelerin görevi tamamlandığında oyun biter.
            finishMenu.SetActive(true);
        }
    }
    public void StepsControl(int refCounter)
    {
        //Referans alınan counter değişkeni, bir seferde ekrana sığan şişe sayısından fazla ise şişelerin hareketini sağlayan metot çağrılır.
        if (refCounter >= refBottlesLength)
        {
            colorConfirmedCount = 0;
            pillConfirmedCount = 0;
            bottleConfirmedCount = 0;
            UpdateTargetPos();
        }
    }
}