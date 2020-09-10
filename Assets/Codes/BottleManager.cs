using UnityEngine;
public class BottleManager : MonoBehaviour
{
    [SerializeField] private LayerMask shelvesGridObjectsLayer = 0;
    private Vector3 shelvesPos;
    private Vector3 bottlePos;
    private Color bottleFirstColor;
    private Color bottleLastColor;
    private Color bottleCapColor;
    private float t = 0;
    private float distance = 0;
    private bool colorChanging;
    public bool colorConfirmed;
    public bool pillConfirmed;
    public bool selectable;
    public int pillCount;

    //Her bir şişede bu Script bulunmaktadır. Şişeye ait değişkenler bulunmakta ve şişenin 3. aşamada raflara taşınması sağlanmaktadır.
    private void Start()
    {
        //Şişenin, hem kapağının hem de camının color değeri ilgili değişkenlere atanır.
        bottleCapColor = transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
        bottleFirstColor = transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color;

        //Raf objesinin pozisyonunun z ekseni alınır ve onun bir şişe yakını değişkene atanır.
        shelvesPos = GameObject.FindGameObjectWithTag("shelvesParent").transform.position;
        shelvesPos.z -= CreateManager.createManagerClass.bottleHorizontalSize;

        //Şişe başlangıçta seçilemez olur.
        selectable = false;
        colorChanging = false;
        colorConfirmed = false;
        pillConfirmed = false;

        //Şişedeki hap sayısı.
        pillCount = 0;
    }
    private void FixedUpdate()
    {
        if (colorChanging)
        {
            //Step 1'de renk değişikliği yumuşak yapılır.
            t += Time.deltaTime * 2;
            transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = Color.Lerp(bottleFirstColor, bottleLastColor, t);
            if(transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color == bottleLastColor)
                colorChanging = false;
        }
    }
    private void OnMouseDown()
    {
        if (selectable)
        {
            //Şişeye tıklandığında şişenin pozisyonu hafızada tutulur. Boyutu küçülür.
            bottlePos = transform.position;
            distance = Mathf.Abs(Camera.main.transform.position.z - shelvesPos.z);
            transform.localScale /= 2;
        }
    }
    private void OnMouseDrag()
    {
        if (selectable)
        {
            //pos pozisyonuna şişe taşınır.
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
            pos.z = shelvesPos.z;
            transform.position = pos;
        }
    }
    private void OnMouseUp()
    {
        if (selectable)
        {
            //Şişedeki sürükleme işlemi bittiğinde mousePosition konumuna bir ray gönderilir.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, shelvesGridObjectsLayer))
            {
                //Rafta bulunan grid objeleri "shelvesGridObjectsLayer" layer değerine sahip
                selectable = false;
                transform.parent = hit.transform.parent;
                transform.localPosition = hit.transform.localPosition;

                //Şişeler doğru raflara koyulduysa 5 puan eklenir.
                if (hit.transform.tag == tag)
                    MoveManager.moveManagerClass.score += 5;

                //Yanlış raflara koyuldu ise 3 puan silinir.
                else
                    MoveManager.moveManagerClass.score -= 3;

                //Rafa koyulan obje sayısı ilgili metoda gönderilir. Bu aşamada görevi yerine getirilen şişe sayısı 1 arttırılır.
                MoveManager.moveManagerClass.StepsControl(++MoveManager.moveManagerClass.bottleConfirmedCount);
                MoveManager.moveManagerClass.completedBottleCount++;

                //Aynı gride tekrar şişe koyulmaması için ilgili pozisyondaki grid objesi silinir.
                Destroy(hit.transform.gameObject);
            }
            else
            {
                //Eğer ki ray grid objesine çarpmaz ise şişe tekrar yerine geçer.
                transform.position = bottlePos;
                transform.localScale *= 2;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("destroyArea"))
        {
            //Bantta bulunan bu trigger ile objeler kamera açısından çıktığında inaktif edilir.
            if(CreateManager.createManagerClass.bottleGroupCount > 1)
                gameObject.SetActive(false);

            //Bu aşamada görevi yerine getirilen şişe sayısı 1 arttırılır.
            MoveManager.moveManagerClass.completedBottleCount++;
        }
        else if (other.tag == "color" && !colorConfirmed)
        {
            //Şişenin rengi Step 1'deki color objectinin rengi ile aynı olur.
            bottleLastColor = other.GetComponent<Renderer>().material.color;
            other.tag = "destroyedObject";
            Destroy(other.gameObject);

            //Şişenin rengi ile kapağın rengi aynı ise score 5 artar.
            if (bottleCapColor == bottleLastColor)
            {
                colorConfirmed = true;
                MoveManager.moveManagerClass.score += 5;
                MoveManager.moveManagerClass.StepsControl(++MoveManager.moveManagerClass.colorConfirmedCount);
            }
            //Farklı ise 3 azalır.
            else
                MoveManager.moveManagerClass.score -= 3;

            //Şişenin rengi biraz saydamlaştırılır.
            bottleLastColor.a = 0.5f;
            colorChanging = true;
        }
    }
    public void PillCountControl(int refCounter)
    {
        //Referans alınan hap sayısı şişenin kapasitesinden fazla ise şişenin kapağı kapatılır.
        if(refCounter >= CreateManager.createManagerClass.bottleCapacity)
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(0).GetComponent<Animator>().enabled = true;
            pillConfirmed = true;
            MoveManager.moveManagerClass.StepsControl(++MoveManager.moveManagerClass.pillConfirmedCount);
        }
    }
}