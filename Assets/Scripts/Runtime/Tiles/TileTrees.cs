public class TileTrees : CTile
{
    public TileTrees()
    {
        this._name = "가로수";
        this._description = "때로는 주변의 자연이 축제를 완성시키기도 합니다.";
        this._tileState = ETileState.Built;
    }

    protected override void Start()
    {
        base.Start();
        Test_TilemapSelector tempClass = FindObjectOfType<Test_TilemapSelector>();
        if (tempClass != null)
        {
            tempClass.Resources.festivalSuccess = tempClass.Resources.festivalSuccess + 1;
        }
    }
}
