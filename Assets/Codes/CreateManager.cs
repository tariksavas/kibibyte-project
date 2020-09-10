using UnityEngine;
public class CreateManager : MonoBehaviour
{
    [SerializeField] private GameObject[] refBottles = null;
    [SerializeField] private GameObject refColorObject = null;
    [SerializeField] private Color[] refColors = null;
    [SerializeField] private GameObject refPetri = null;
    [SerializeField] private GameObject refShelves = null;
    [SerializeField] private GameObject refConveyorBelt = null;
    [SerializeField] private GameObject colorObjectsParent = null;
    [SerializeField] private GameObject petriObjectParent = null;
    [SerializeField] private GameObject shelvesObjectParent = null;
    [SerializeField] private GameObject[] pill = null;
    private float colorHorizontalSize = 0;
    public int bottleGroupCount = 2;
    public int bottleCapacity = 4;
    public int refBottlesLength = 0;
    public float bottleHorizontalSize = 0;
    public float horizontal = 0;
    public bool translateCompleted;
    public Animator beltAnimator = null;
    public static CreateManager createManagerClass;

    //Bu script bottleParent objesinde bulunur. 
    //Sahnedeki tüm objelerin oluşturulması bu script ile sağlanır.
    private void Awake()
    {
        createManagerClass = this;
        refBottlesLength = refBottles.Length;

        //Ekran ölçülerine göre değişkenlere ilgili değerler atanır.
        float orthographicSizeY = (float)Camera.main.orthographicSize;
        float orthographicSizeX = orthographicSizeY * ((float)Screen.width / (float)Screen.height);

        horizontal = orthographicSizeX;
        colorHorizontalSize = horizontal / ((float)refColors.Length / 2);
        bottleHorizontalSize = horizontal / ((float)refBottlesLength / 2);

        translateCompleted = false;
        CreateConveyorBelt();
        CreateColorObjects();
        CreatePetri();
        CreateShelves();
    }
    private void Start()
    {
        //Şişeler, sahnedeki tüm objeler oluştuktan sonra oluşur.
        CreateBottles();
    }
    private void Update()
    {
        if (petriObjectParent.transform.position.x == 0 && !translateCompleted)
        {
            //Tepsinin kamera önüne gelmesiyle haplar oluşturulur.(Step 2)
            translateCompleted = true;
            FindPills();
        }
    }
    private void CreateConveyorBelt()
    {
        //Referans alınan bant objesini sahnede oluşturur. Ekran ölçülerine göre büyüklüğü ayarlanır.
        //Bant objesinin hareket animasyonunun bulunduğu animator componenti bir değişkene atanır.
        GameObject conveyorBeltCopy = Instantiate(refConveyorBelt);
        conveyorBeltCopy.transform.position = new Vector3(0, -1, -8);
        conveyorBeltCopy.transform.localScale *= horizontal;
        beltAnimator = conveyorBeltCopy.GetComponent<Animator>();
    }
    private void CreateColorObjects()
    {
        for(int i = 0; i < refColors.Length; i++)
        {
            //Editörden girilen renkler referans alınan color objesinin clone'una atanır.
            GameObject colorCopy = Instantiate(refColorObject, colorObjectsParent.transform);
            colorCopy.GetComponent<Renderer>().material.color = refColors[i];
            //Ekran ölçülerine göre pozisyon ve boyut ayarları yapılır.
            colorCopy.transform.position = new Vector3((i * colorHorizontalSize) - horizontal + (horizontal / refColors.Length), 0.5f, -8);
            colorCopy.transform.localScale *= colorHorizontalSize;
        }
    }
    private void CreatePetri()
    {
        //Referans alınan tepsi objesini sahnede oluşturur. Pozisyon ve boyut ayarları yapılır.
        GameObject petriCopy = Instantiate(refPetri, petriObjectParent.transform);
        petriCopy.transform.localPosition = new Vector3(0, 0, 0);
        Vector3 petriScale = petriCopy.transform.localScale * horizontal;

        if (bottleCapacity < 4)
            petriScale.y /= (4 * refBottlesLength);
        else
            petriScale.y /= (bottleCapacity * refBottlesLength);
        
        petriCopy.transform.localScale = petriScale;
    }
    private void CreateShelves()
    {
        //Referans alınan raf objesini sahnede oluşturur. Pozisyon ve boyut ayarları yapılır.
        GameObject shelvesCopy = Instantiate(refShelves, shelvesObjectParent.transform);
        shelvesCopy.transform.localPosition = new Vector3(0, 0, 0);
        shelvesCopy.transform.localScale *= horizontal;
    }
    private GameObject[] RandomSet(GameObject[] refObjects)
    {
        //Referans alınan GameObject dizisini rastgele sıralar.
        int randomIndex = 0;
        int refObjectsLength = refObjects.Length;
        GameObject[] newRefObjects = new GameObject[refObjectsLength];
        for (int i = 0; i < refObjects.Length; i++)
        {
            randomIndex = Random.Range(0, refObjectsLength);
            newRefObjects[i] = refObjects[randomIndex];

            //Rastgele bulunan indis değerindeki obje newRefObjects dizisine atandıktan sonra
            //bu indis değerinden sonra gelen objeler bir birim öne atanır ve dizinin boyutu 1 azaltılır.
            for (int j = randomIndex; j < refObjectsLength - 1; j++)
                refObjects[j] = refObjects[j + 1];

            refObjectsLength--;
        }
        return newRefObjects;
    }
    private void CreateBottles()
    {
        refBottles = RandomSet(refBottles);
        //Şişeler, boyutları düzenlenir ve bu metotta sahnede oluşturulur. 
        //MoveManager scriptinde bulunan bottles GameObject dizisine bu şişeler atanır.
        MoveManager.moveManagerClass.bottles = new GameObject[refBottlesLength * bottleGroupCount];
        int bottlesIndex = 0;
        for(int i = 0; i < bottleGroupCount; i++)
        {
            //Editörden alınan değerlere göre şişe clone'ları oluşturulur.
            for(int j = 0; j < refBottlesLength; j++)
            {
                MoveManager.moveManagerClass.bottles[bottlesIndex] = Instantiate(refBottles[j], gameObject.transform);
                MoveManager.moveManagerClass.bottles[bottlesIndex].transform.localScale *= bottleHorizontalSize;
                MoveManager.moveManagerClass.bottles[bottlesIndex].SetActive(false);
                bottlesIndex++;
            }
        }
        //Şişeler oluşturulduktan sonra MoveManager scriptindeki bant hareketini sağlayan metot çağırılır.
        MoveManager.moveManagerClass.UpdateTargetPos();
    }
    private void FindPills()
    {
        //Şişelerin kapasitesi kadar tüm hap indisleri DropPill metotuna gönderilir.
        for (int i = 0; i < refBottlesLength; i++)
        {
            for (int j = 0; j < pill.Length; j++)
            {
                if(refBottles[i].transform.GetChild(0).tag == pill[j].tag)
                {
                    for(int k = 0; k < bottleCapacity; k++)
                    {
                        DropPill(j); 
                    }
                }
            }
        }
    }
    public void FindPill(string objectTag)
    {
        //Referans alınan hapın tagine göre sahnede tekrar oluşturur.
        for (int i = 0; i < pill.Length; i++)
        {
            if (pill[i].tag == objectTag)
            {
                DropPill(i);
            }
        }
    }
    private void DropPill(int index)
    {
        //pill dizisinden referans alınan hap indisini sahnede oluşturur.
        GameObject pillCopy = Instantiate(pill[index]);
        Vector3 dropPos = petriObjectParent.transform.position;
        dropPos.y += 2;
        pillCopy.transform.position = dropPos;

        //Hap boyutları da şişe sayılarına ve kapasitelerine göre değişmektedir. Maksimum boyut ile sınırlandırılmıştır.
        if (bottleCapacity < 4)
            pillCopy.transform.localScale *= (float)(bottleHorizontalSize / 4);
        else
            pillCopy.transform.localScale *= (float)(bottleHorizontalSize / bottleCapacity);
    }
}