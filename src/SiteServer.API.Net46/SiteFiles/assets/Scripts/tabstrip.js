
function Tab_OnMouseOver(eventTarget)
{
	if(eventTarget.className != "tab hover selected")
	{
		eventTarget.className = "tab hover"
	}
}

function Tab_OnMouseOut(eventTarget)
{
	if(eventTarget.className != "tab hover selected")
	{
		eventTarget.className = "tab"
	}
}

function Tab_OnSelectServerClick(eventTarget,TabPageID)
{
		eventTarget.parentNode.parentNode.childNodes[0].value = TabPageID;
}

function Tab_OnSelectClientClick(eventTarget,TabPageID)
{
		
		RootTabDiv = eventTarget.parentNode.parentNode.childNodes[1];
		RootTabPageDiv = eventTarget.parentNode.parentNode.childNodes[2];
		//alert(RootTabDiv);
		
		for(i=0;i<RootTabDiv.childNodes.length;i++)
		{
			RootTabDiv.childNodes[i].className = "tab";
		}
		
		for(i=0;i<RootTabPageDiv.childNodes.length;i++)
		{
			RootTabPageDiv.childNodes[i].style.display = "none";
		}
       
        //alert(TabPageID);
		document.getElementById(TabPageID).style.display = 'block';
		document.getElementById(TabPageID+"_H2").className = 'tab hover selected';
		eventTarget.parentNode.parentNode.childNodes[0].value = TabPageID;
		
}
