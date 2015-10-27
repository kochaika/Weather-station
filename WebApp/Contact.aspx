<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="WebApp.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Our contact page.</h2>
    </hgroup>

    <section class="contact">
        <h3>Architect</h3>
        <p>
            <span><a href="https://vk.com/id39891456"><img src="Images/contact_k.png" alt="Эмблема"></a><br>
                <a href="mailto:pro100kot14@gmail.com">Чайка Константин</a></span>
        </p>
        <h3>Developer</h3>
        <p>
            <span><a href="https://vk.com/vladspam"><img src="Images/contact_v.png" alt="Эмблема"></a><br>
                <a href="mailto:dudovlad@gmail.com">Дудов Владислав</a></span>
        </p>
        <h3>Tester</h3>
        <p>
            <span><a href="https://vk.com/fahrenheit001"><img src="Images/contact_i.png" alt="Эмблема"></a><br>
                <a href="mailto:vertileckii.ilya@gmail.com">Вертилецкий Илья</a></span>
        </p>
        <h3>Project manager</h3>
        <p>
            <span><a href="https://vk.com/razmn"><img src="Images/contact_n.png" alt="Эмблема"><br></a>
            <a href="mailto:natalyrazzm@gmail.com">Размочаева Наталья</a></span>
        </p>
    </section>

   <!-- <section class="contact">
        <header>
            <h3>Address:</h3>
        </header>
        <p>
            One Microsoft Way<br />
            Redmond, WA 98052-6399
        </p>
    </section>-->
</asp:Content>