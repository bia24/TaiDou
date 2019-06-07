# TaiDou
An MMOARPG game using Unity3D
## Summary
This is a relatively complete project with the following modules：
* Network
  * Using PhotonServer(3.4.9.2931) with Mysql(5.7.26) to transfer and store data.
  * LitJson is used for transformation.
  * Find /TaidouServer and complie the .dll into base directory which PhotonServer needed to start.
  * How to start PhotonServer? See more:[PhotonServer](https://www.photonengine.com/).
* UI
  * Base on NGUI
  * Using HudText for battle UI
* Task
  * AutoMove with the Navigation system to reach Npc and Raid entry.
* Knapsack
  * Include item dressing up/down,solding,leveling up,eating.
* Skill
* GameControl
* Raid Battle
  * Using the client-Server mode to synchronize data.All logic calculation is at local client not in server.
  * One client must be choose as master-client to synchronize enemise action.
## Game Screenshot
![](https://github.com/bia24/TaiDou/blob/master/GameScreenShot.png)
  
   
