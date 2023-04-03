using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class GroundSizeController : MonoBehaviour {

	public GameObject sideGround;
	public GameObject midGround;

	private GameObject leftSide;
	private GameObject rightSide;

	private GameObject mid;
	private Vector3 midBoundSize;
	private MeshFilter midMeshFilter;
	private MeshRenderer midMeshRender;
	private float midHalfSize;
	//private Material midGroundMaterial;

	private Vector3 leftBoundSize;
	private MeshFilter lefSideMeshFilter;
	private MeshRenderer leftSideMeshRender;
	//private Material leftSideMaterial;

	private MeshRenderer rightSideRender;
	//private Material rightSideMaterial;

	private BoxCollider  boxCollider;
	private int childCount;

	public int length=1;
	public bool isClear=false;
	public bool generate=false;
	public bool combineMeshNow=false;

	private GameObject combineObj;
	public Material tileGroundMat;

	private Action GenerateGroundComplete;
	public event Action OnGenerateGroundComplete{
		add{GenerateGroundComplete+=value;}
		remove{GenerateGroundComplete-=value;}
	}

	// Use this for initialization
	void Start (){
		//originalPosition = this.gameObject.transform.position;
		if(Application.isPlaying){
			isClear	= false;
			generate =false;
			combineMeshNow =false;
		}
		//CheckChildMeshRenderer(this.gameObject);
	}

	private void ClearChild(){
		int cnt = this.gameObject.transform.childCount-1;
		for( int index=cnt;index>=0;index--){
			Transform child = this.gameObject.transform.GetChild(index);
			DestroyImmediate(child.gameObject);
		}

		leftSide = null;
		mid = null;
		rightSide = null;
		//midGroundMaterial = null;
		//leftSideMaterial = null;
		//rightSideMaterial = null;
		boxCollider = null;
		childCount = this.gameObject.transform.childCount;
	}
	
	// Update is called once per frame
	void Update (){
		if(Application.isPlaying)return;

		if(isClear){
			if(this.gameObject.transform.childCount> 0){
				ClearChild();
			}
		}

		if(generate && childCount <3){
			GenerateGround();
		}

		if(combineMeshNow){
			CombineMesh();
		}
	}

	private void CombineMesh(){
		if(combineObj!=null){
			DestroyImmediate(combineObj.gameObject);
		}

		combineObj = new GameObject();
		combineObj.name ="combineGround";
		combineObj.transform.parent = this.gameObject.transform;

		if(!combineObj.gameObject.transform.GetComponent<MeshFilter>()){
			combineObj.gameObject.AddComponent<MeshFilter>();
		}

		if(!combineObj.transform.GetComponent<MeshRenderer>()){
			combineObj.gameObject.AddComponent<MeshRenderer>();
		}

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}
		combineObj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		combineObj.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
		combineObj.transform.gameObject.SetActive(true);
		//combineObj.transform.GetComponent<MeshRenderer>().sharedMaterial = midGroundMaterial;
		combineObj.transform.GetComponent<MeshRenderer>().sharedMaterial = tileGroundMat;
		combineObj.transform.GetComponent<MeshRenderer>().renderer.sharedMaterial.mainTextureScale = new Vector2(combineObj.gameObject.transform.localScale.x,1);

		combineMeshNow =false;
		isClear = false;
		generate =false;

		//DestroyImmediate(rightSide.gameObject);
		//DestroyImmediate(mid.gameObject);
		//DestroyImmediate(leftSide.gameObject);
	}

	private void LoadTexture(){
		//midGroundMaterial = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
		//midGroundMaterial.mainTexture = Resources.Load("_Models/grounds2/tile01b") as Texture2D;	
		//midMeshRender.renderer.sharedMaterial = midGroundMaterial;
		midMeshRender.renderer.sharedMaterial = tileGroundMat;
		midMeshRender.renderer.sharedMaterial.mainTextureScale = new Vector2(length * 0.5f,1);
		
		//leftSideMaterial = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
		//leftSideMaterial.mainTexture =Resources.Load("_Models/grounds2/tile01b") as Texture2D;
		//leftSideMeshRender.renderer.sharedMaterial = leftSideMaterial;
		leftSideMeshRender.renderer.sharedMaterial = tileGroundMat;



		//rightSideMaterial = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
		//rightSideMaterial.mainTexture =Resources.Load("_Models/grounds2/tile01b") as Texture2D;
		//rightSideRender.renderer.sharedMaterial = rightSideMaterial;
		rightSideRender.renderer.sharedMaterial = tileGroundMat;
	}

	private void CheckChildMeshRenderer( GameObject obj ){
		foreach( Transform child in obj.transform ){
			MeshRenderer childRenderer = child.gameObject.GetComponent<MeshRenderer>();
			if(childRenderer!=null){
				Material newMat = new Material(Shader.Find("Mobile/Unlit (Supports Lightmap)"));
				newMat.mainTexture =Resources.Load("_Models/grounds2/tile01b") as Texture2D;
				childRenderer.renderer.sharedMaterial = newMat;
				childRenderer.renderer.sharedMaterial.mainTextureScale = new Vector2(child.gameObject.transform.localScale.x,1);
			}
			CheckChildMeshRenderer(child.gameObject);
		}
	}

	private void GenerateGround(){
		if(leftSide==null){
			leftSide = Instantiate(sideGround, this.gameObject.transform.position,Quaternion.Euler(0,180,0)) as GameObject;
			leftSide.gameObject.transform.parent = this.gameObject.transform;
			leftSide.name = "leftSide";
		}
		
		leftSideMeshRender = leftSide.GetComponentInChildren<MeshRenderer>();		
		lefSideMeshFilter = leftSide.GetComponentInChildren<MeshFilter>();
		leftBoundSize = lefSideMeshFilter.sharedMesh.bounds.size;

		//Debug.Log("side added!");		
		if(mid==null){
			mid = Instantiate(midGround, this.gameObject.transform.position,Quaternion.Euler(0,180,0)) as GameObject;
			mid.gameObject.transform.parent = this.gameObject.transform;
			mid.name = "middle";
		}
		
		Vector3 tempMidScale = mid.gameObject.transform.localScale;
		tempMidScale.x = length;
		mid.gameObject.transform.localScale = tempMidScale;
		
		midMeshFilter = mid.GetComponentInChildren<MeshFilter>();
		midMeshRender = mid.GetComponentInChildren<MeshRenderer>();
		midBoundSize = midMeshFilter.sharedMesh.bounds.size;

		float newMidBoundSize = midBoundSize.x * length;
		float newMidHalfBoundSize = newMidBoundSize * 0.5f;
		float leftSideOffsetX = leftBoundSize.x * 0.06f;

		//Debug.Log("midBoundSize x " + midBoundSize.x);
		//Debug.Log("newMidBoundSize x " + newMidBoundSize);

		midHalfSize = midBoundSize.x * 0.5f;
		//midMeshRender.renderer.sharedMaterial = midGroundMaterial;


		//if(leftBoundSize!=null){
			Vector3 midTempPosition = mid.gameObject.transform.position;
			midTempPosition.x = (leftSide.gameObject.transform.position.x + newMidHalfBoundSize) - (leftBoundSize.x + leftSideOffsetX);
			mid.gameObject.transform.position = midTempPosition;
			Debug.Log("reposition mid");
		//}	


		if(rightSide==null){
			rightSide = Instantiate(sideGround, this.gameObject.transform.position,Quaternion.Euler(0,180,0)) as GameObject;
			rightSide.gameObject.transform.parent = this.gameObject.transform;
			rightSide.name = "rightSide";
		}
		
		rightSideRender = rightSide.GetComponentInChildren<MeshRenderer>();		
		Vector3 tempScale =rightSide.gameObject.transform.localScale;
		tempScale.x = -1;
		rightSide.gameObject.transform.localScale = tempScale;


		//if(newMidBoundSize!=null){
			Vector3 rightTempPosition = rightSide.gameObject.transform.position;
			rightTempPosition.x = mid.gameObject.transform.position.x + (newMidBoundSize * 0.5f) - (leftBoundSize.x + leftSideOffsetX);
			rightSide.gameObject.transform.position = rightTempPosition;
			Debug.Log("reposition right");
		//}

		if(boxCollider == null){
			if(this.gameObject.GetComponent<BoxCollider>()==null){
				this.gameObject.AddComponent<BoxCollider>();
			}
			boxCollider = this.gameObject.GetComponent<BoxCollider>();
		}

		float totalSizeX = (leftBoundSize.x * 2 ) + (midBoundSize.x * (mid.gameObject.transform.localScale.x));
		boxCollider.size= new Vector3(totalSizeX,leftBoundSize.y,leftBoundSize.z);
		boxCollider.center = new Vector3((midHalfSize * (mid.gameObject.transform.localScale.x - 1)) ,-(leftBoundSize.y *0.25f),0);
		//Debug.Log("size totalSize " + totalSizeX);

		LoadTexture();

		if(null!=GenerateGroundComplete){
			GenerateGroundComplete();
		}
	}
}
