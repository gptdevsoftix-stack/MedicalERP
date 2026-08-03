//var baseurl="../../html/vertical-menu-";

function fetchmenu(){
  return $.ajax({
    contentType: 'application/json; charset=utf-8',
    type: 'GET',
    url: "https://localhost:7256/api/Menu/Menudata",
    success: function(result){
      console.log("menu list:", result);
      var menuhtml="";
      if(result==null){
      }
      else{
        for(var i=0;i<result.length;i++){
          menuhtml+='<li class="menu-item">';
          menuhtml+='<a href="'+result[i].m_url+'" class="menu-link">';
          menuhtml+='<i class="menu-icon tf-icons ti ti-messages"></i>';
          menuhtml+='<div data-i18n="'+result[i].m_name+'">'+result[i].m_name+'</div>';
          menuhtml+='</a>';
          menuhtml+='</li>';
        }
       $('.menulist').html(menuhtml);
          
      }
  },
     error: function(xhr, status, error) {
    console.error("Error sending data:", error);
    // Handle error response here
  }
  
  });
}

function fetchrolelist(){
  return $.ajax({
    contentType: 'application/json; charset=utf-8',
    type: 'GET',
    url: "https://localhost:7256/api/Roles/Roles",
    success: function(result){
      console.log("menu list:", result);
      var menuhtml="";
      if(result==null){
      }
      else{
        for(var i=0;i<result.length;i++){
          menuhtml+= '<option value="'+result[i].id+'">'+result[i].name+'</option> ';
         
        }
       $('.Rolelist').html(menuhtml);
          
      }
  },
     error: function(xhr, status, error) {
    console.error("Error sending data:", error);
    // Handle error response here
  }
  
  });
}
$(document).ready(function(){

  var x = localStorage.getItem("userdata");
  if(x !=[])
{
  x=JSON.parse(x);
  $(".Username").text(x[0].id);
  }
  fetchmenu();
  $("#signup").click(function(){
    var l_name = $('#l_name').val();
    var f_name = $('#f_name').val();
    var e_mail = $('#e_mail').val();
    var pass = $('#pass').val();
    var c_pass = $('#c_pass').val();
    if( l_name == null || f_name == null || e_mail == null  || pass == null || c_pass == null) {
      alert('Please fill out all fields');
      return false;
    }

if(pass != c_pass){
   alert("Passwords do not match");
  return false;
  
}



var registers={
  f_name:f_name,
  l_name:l_name,
  e_mail:e_mail,
  pass:pass,
  c_pass:c_pass
}

$.ajax({
  contentType: 'application/json; charset=utf-8',
 
  data: JSON.stringify(registers),
  type: 'Post',
  url: "https://localhost:7256/api/Auth/alldataregister",
  success: function(result){
    console.log("Data sent successfully:", result);
    if(result==null){
        alert("data not saved");
        
    }
    else{
        alert("data saved");
        // window.location.href="../../html/vertical-menu-template/auth-login-cover.html";
    }
},
   error: function(xhr, status, error) {
  alert(" Data not sent");
  // Handle error response here
}

});
    
  });

  $("#login").click(function(){
    var e_mail = $('#email').val();
    var pass = $('#pass').val();
    if(e_mail == null  || pass == null) {
      alert('Please fill out all fields');
      return false;
    }



$.ajax({
  contentType: 'application/json; charset=utf-8',
  type: 'GET',
  url: "https://localhost:7256/api/Auth/logindata?e_mail="+e_mail+"&pass="+pass+"",
  success: function(result){
    console.log("Login successfully:", result);
    if(result==null){
        alert("data not saved");
    }
    else{
        alert("data saved");
        localStorage.setItem('userdata' , JSON.stringify(result));
        window.location.href="../../html/vertical-menu-template/Product.html";
    }
},
   error: function(xhr, status, error) {
    alert(" Data not sent");
  // Handle error response here
}

});
    
  });
});
///menu//




