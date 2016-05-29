namespace ViewModels

open FSharp.ViewModule

type FolderListViewModel(store : Store) as this =
    inherit ViewModelBase()

    let inboxFolder = store.InboxFolder
    let selectedFolder = store.SelectedFolder

    do
        inboxFolder.Publish.Add(fun _ ->
            this.RaisePropertyChanged(<@ this.List @>))

    member __.List with get() = inboxFolder.Value

    member this.SelectFolderCommand = this.Factory.CommandSyncParam(selectedFolder.Trigger)
