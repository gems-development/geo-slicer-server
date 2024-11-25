namespace DomainModels
{
    public class FragmentWithNonRenderingBorder<TFragmentType, TNonRenderingBorderType>
    {
        public TFragmentType Fragment { get;  set; }
        public TNonRenderingBorderType NonRenderingBorder { get;  set; }

        public FragmentWithNonRenderingBorder() {}

        public FragmentWithNonRenderingBorder(TFragmentType fragment, TNonRenderingBorderType nonRenderingBorder)
        {
            Fragment = fragment;
            NonRenderingBorder = nonRenderingBorder;
        }
    }
}