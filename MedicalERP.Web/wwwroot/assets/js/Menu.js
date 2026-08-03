$(document).ready(function(){
    $("#Menu").click(function(){
      var m_name = $('#m_name').val();
      var m_url = $('#m_url').val();
      var m_role = $('#m_role').val();
      var m_child = $('#m_child').val();
      var activeinacrive = $('.activeinactive').val();
      if( m_name == null || m_url == null || m_role == null  || m_child == null|| activeinacrive == null ) {
        alert('Please fill out all fields');
        return false;
      }
  
  
  
  var Menu ={
    m_name:m_name,
    m_url:m_url,
    m_role:m_role,
    m_child:m_child,
  
  }
  
  $.ajax({
    contentType: 'application/json; charset=utf-8',
   
    data: JSON.stringify(Menu),
    type: 'Post',
    url: "https://localhost:7256/api/Menu/allMenuCreate ",
    success: function(result){
      console.log("Data sent successfully:", result);
      if(result!=null){
          alert("data saved");
          fetchmenu();
      }
      else{
          alert("data not saved");
        
          // window.location.href="../../html/vertical-menu-template/auth-login-cover.html";
      }
  },
     error: function(xhr, status, error) {
    alert(" Data not sent");
    // Handle error response here
  }
  
  });
      
    });

    $.ajax({
      contentType: 'application/json; charset=utf-8',
     
     
      type: 'GET',
      url: "https://localhost:7256/api/Roles/Roles",
      success: function(result){
        console.log("ROLE LIST:", result);
        if(result!=null){
          var rolehtml="";
          for(var i=0;i<result.length;i++)
            {
              rolehtml+="<option value='"+result[i].id+"'>"+result[i].name+"</option>";
              
            }
            $(".myrolegroup").html(rolehtml);
        }
        else{
        alert("data not foudn");
        }
    },
       error: function(xhr, status, error) {
      alert(" Data not sent");
      // Handle error response here
    }
  });
    
    //$(".myselectrole2")

});
$.ajax({
  contentType: 'application/json; charset=utf-8',
 
 
  type: 'GET',
  url: "https://localhost:7256/api/Roles/Roles",
  success: function(result){
    console.log("ROLE LIST:", result);
    if(result!=null){
      var rolehtml="";
      for(var i=0;i<result.length;i++)
        {
          rolehtml+="<option value='"+result[i].id+"'>"+result[i].name+"</option>";
          
        }
        $(".myrolegroup").html(rolehtml);
    }
    else{
    alert("data not foudn");
    }
},
   error: function(xhr, status, error) {
  alert(" Data not sent");
  // Handle error response here
}
});

