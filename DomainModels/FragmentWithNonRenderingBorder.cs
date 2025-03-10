namespace DomainModels
{
    public class FragmentWithNonRenderingBorder<TFragmentType, TNonRenderingBorderType>
    {
        public TFragmentType Fragment { get;  set; } = default!;
        public TNonRenderingBorderType NonRenderingBorder { get;  set; } = default!;

        public FragmentWithNonRenderingBorder() {}

        public FragmentWithNonRenderingBorder(TFragmentType fragment, TNonRenderingBorderType nonRenderingBorder)
        {
            Fragment = fragment;
            NonRenderingBorder = nonRenderingBorder;
        }
    }
}