using UnityEngine;
using System;

public class UgsAdapter : MonoBehaviour
{
	public Action<string> OnConnected;
	public void onConnected(string status)
	{
		//Debug.Log("GpgAdapter.onConnected: " + status);
		OnConnected(status);
	}
	
	public Action<string> OnDisconnected;
	public void onDisconnected(string status)
	{
		//Debug.Log("GpgAdapter.onDisconnected: " + status);
		OnDisconnected(status);
	}
	
	public Action<string> OnConnectionFailed;
	public void onConnectionFailed(string status)
	{
		//Debug.Log("GpgAdapter.onConnectionFailed: " + status);
		OnConnectionFailed(status);
	}
	
	public Action<string> OnImageLoaded;
	public void onImageLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onImageLoaded: " + status);
		OnImageLoaded(status);
	}
	
	public Action<string> OnAchievementsLoaded;
	public void onAchievementsLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onAchievementsLoaded: " + status);
		OnAchievementsLoaded(status);
	}
	
	public Action<string> OnLeaderboardMetadataLoaded;
	public void onLeaderboardMetadataLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onLeaderboardMetadataLoaded: " + status);
		OnLeaderboardMetadataLoaded(status);
	}

	public Action<string> OnLeaderboardScoresLoaded;
	public void onLeaderboardScoresLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onLeaderboardScoresLoaded: " + status);
		OnLeaderboardScoresLoaded(status);
	}
	
	public Action<string> OnGamesLoaded;
	public void onGamesLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onGamesLoaded: " + status);
		OnGamesLoaded(status);
	}
	
	public Action<string> OnStatesListLoaded;
	public void onStatesListLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onStatesListLoaded: " + status);
		OnStatesListLoaded(status);
	}
	
	public Action<string> OnStateLoaded;
	public void onStateLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onStateLoaded: " + status);
		OnStateLoaded(status);
	}
	
	public Action<string> OnStateSaved;
	public void onStateSaved(string status)
	{
		//Debug.Log("GpgAdapter.onStateSaved: " + status);
		OnStateSaved(status);
	}
	
	public Action<string> OnStateConflicted;
	public void onStateConflicted(string status)
	{
		//Debug.Log("GpgAdapter.onStateConflicted: " + status);
		OnStateConflicted(status);
	}
	
	public Action<string> OnStateDeleted;
	public void onStateDeleted(string status)
	{
		//Debug.Log("GpgAdapter.onStateDeleted: " + status);
		OnStateDeleted(status);
	}

	public Action<string> OnStateResolved;
	public void onStateResolved(string status)
	{
		//Debug.Log("GpgAdapter.onStateResolved: " + status);
		OnStateResolved(status);
	}
	
	public Action<string> OnPlayersSelected;
	public void onPlayersSelected(string status)
	{
		//Debug.Log("GpgAdapter.onPlayersSelected: " + status);
		OnPlayersSelected(status);
	}
	
	public Action<string> OnInvitationSelected;
	public void onInvitationSelected(string status)
	{
		//Debug.Log("GpgAdapter.onInvitationSelected: " + status);
		OnInvitationSelected(status);
	}
	
	public Action<string> OnInvitablePlayersLoaded;
	public void onInvitablePlayersLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onInvitablePlayersLoaded: " + status);
		OnInvitablePlayersLoaded(status);
	}
	
	public Action<string> OnMoreInvitablePlayersLoaded;
	public void onMoreInvitablePlayersLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onMoreInvitablePlayersLoaded: " + status);
		OnMoreInvitablePlayersLoaded(status);
	}
	
	public Action<string> OnInvitationReceived;
	public void onInvitationReceived(string status)
	{
		//Debug.Log("GpgAdapter.onInvitationReceived: " + status);
		OnInvitationReceived(status);
	}
	
	public Action<string> OnInvitationsLoaded;
	public void onInvitationsLoaded(string status)
	{
		//Debug.Log("GpgAdapter.onInvitationsLoaded: " + status);
		OnInvitationsLoaded(status);
	}
	
	public Action<string> OnRoomJoined;
	public void onRoomJoined(string status)
	{
		//Debug.Log("GpgAdapter.onRoomJoined: " + status);
		OnRoomJoined(status);
	}
	
	public Action<string> OnRoomLeft;
	public void onRoomLeft(string status)
	{
		//Debug.Log("GpgAdapter.onRoomLeft: " + status);
		OnRoomLeft(status);
	}
	
	public Action<string> OnRoomConnected;
	public void onRoomConnected(string status)
	{
		//Debug.Log("GpgAdapter.onRoomConnected: " + status);
		OnRoomConnected(status);
	}
	
	public Action<string> OnRoomCreated;
	public void onRoomCreated(string status)
	{
		//Debug.Log("GpgAdapter.onRoomCreated: " + status);
		OnRoomCreated(status);
	}
	
	public Action<string> OnRealTimeMessageSent;
	public void onRealTimeMessageSent(string status)
	{
		//Debug.Log("GpgAdapter.onRealTimeMessageSent: " + status);
		OnRealTimeMessageSent(status);
	}
	
	public Action<string> OnRealTimeMessageReceived;
	public void onRealTimeMessageReceived(string status)
	{
		//Debug.Log("GpgAdapter.onRealTimeMessageReceived: " + status);
		OnRealTimeMessageReceived(status);
	}
	
	public Action<string> OnConnectedToRoom;
	public void onConnectedToRoom(string status)
	{
		//Debug.Log("GpgAdapter.onConnectedToRoom: " + status);
		OnConnectedToRoom(status);
	}
	
	public Action<string> OnDisconnectedFromRoom;
	public void onDisconnectedFromRoom(string status)
	{
		//Debug.Log("GpgAdapter.onDisconnectedFromRoom: " + status);
		OnDisconnectedFromRoom(status);
	}
	
	public Action<string> OnPeerDeclined;
	public void onPeerDeclined(string status)
	{
		//Debug.Log("GpgAdapter.onPeerDeclined: " + status);
		OnPeerDeclined(status);
	}
	
	public Action<string> OnPeerInvitedToRoom;
	public void onPeerInvitedToRoom(string status)
	{
		//Debug.Log("GpgAdapter.onPeerInvitedToRoom: " + status);
		OnPeerInvitedToRoom(status);
	}
	
	public Action<string> OnPeerJoined;
	public void onPeerJoined(string status)
	{
		//Debug.Log("GpgAdapter.onPeerJoined: " + status);
		OnPeerJoined(status);
	}
	
	public Action<string> OnPeerLeft;
	public void onPeerLeft(string status)
	{
		//Debug.Log("GpgAdapter.onPeerLeft: " + status);
		OnPeerLeft(status);
	}
	
	public Action<string> OnPeersConnected;
	public void onPeersConnected(string status)
	{
		//Debug.Log("GpgAdapter.onPeersConnected: " + status);
		OnPeersConnected(status);
	}
	
	public Action<string> OnPeersDisconnected;
	public void onPeersDisconnected(string status)
	{
		//Debug.Log("GpgAdapter.onPeersDisconnected: " + status);
		OnPeersDisconnected(status);
	}
	
	public Action<string> OnRoomAutoMatching;
	public void onRoomAutoMatching(string status)
	{
		//Debug.Log("GpgAdapter.onRoomAutoMatching: " + status);
		OnRoomAutoMatching(status);
	}
	
	public Action<string> OnRoomConnecting;
	public void onRoomConnecting(string status)
	{
		//Debug.Log("GpgAdapter.onRoomConnecting: " + status);
		OnRoomConnecting(status);
	}
	
	public Action<string> OnWaitingRoomFinished;
	public void onWaitingRoomFinished(string status)
	{
		//Debug.Log("GpgAdapter.onWaitingRoomFinished: " + status);
		OnWaitingRoomFinished(status);
	}
	
	public Action<string> OnWaitingRoomCanceled;
	public void onWaitingRoomCanceled(string status)
	{
		//Debug.Log("GpgAdapter.onWaitingRoomCanceled: " + status);
		OnWaitingRoomCanceled(status);
	}
	
	public Action<string> OnWaitingRoomLeft;
	public void onWaitingRoomLeft(string status)
	{
		//Debug.Log("GpgAdapter.onWaitingRoomLeft: " + status);
		OnWaitingRoomLeft(status);
	}
}
