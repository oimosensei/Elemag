using ObservableCollections;
using UnityEngine;

internal sealed class RemoveFilter : ISynchronizedViewFilter<ScoreBoardItem, ScoreBoardItemView>
{
    public bool IsMatch(ScoreBoardItem value, ScoreBoardItemView view)
    {
        return true;
    }

    public void WhenTrue(ScoreBoardItem value, ScoreBoardItemView view)
    {
    }

    public void WhenFalse(ScoreBoardItem value, ScoreBoardItemView view)
    {
    }

    public void OnCollectionChanged(ChangedKind changedKind,
        ScoreBoardItem value,
        ScoreBoardItemView view,
        in NotifyCollectionChangedEventArgs<ScoreBoardItem> eventArgs)
    {
        if (changedKind == ChangedKind.Remove)
        {
            if (view != null) Object.Destroy(view.gameObject);
        }
    }
}