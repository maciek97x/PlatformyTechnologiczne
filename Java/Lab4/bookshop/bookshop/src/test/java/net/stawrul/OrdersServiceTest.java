package net.stawrul;

import net.stawrul.controllers.BooksController;
import net.stawrul.model.Book;
import net.stawrul.model.Order;
import net.stawrul.services.BooksService;
import net.stawrul.services.OrdersService;
import net.stawrul.services.exceptions.EmptyOrderException;
import net.stawrul.services.exceptions.OutOfStockException;
import net.stawrul.services.exceptions.TooManyProductsException;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.runners.MockitoJUnitRunner;
import org.springframework.web.util.UriComponentsBuilder;

import javax.persistence.EntityManager;

import java.util.ArrayList;
import java.util.List;

import static org.junit.Assert.assertEquals;
import static org.mockito.Mockito.times;

@RunWith(MockitoJUnitRunner.class)
public class OrdersServiceTest {

    @Mock
    EntityManager em;

    @Test(expected = OutOfStockException.class)
    public void whenOrderedBookNotAvailable_placeOrderThrowsOutOfStockEx() {
        //Arrange
        Order order = new Order();
        Book book = new Book();
        book.setAmount(0);
        order.getBooks().add(book);

        Mockito.when(em.find(Book.class, book.getId())).thenReturn(book);

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert - exception expected
    }

    @Test
    public void whenLastOfOrderedBooksNotAvailable_placeOrderRevertChangesToRestBooks() {
        //Arrange
        Order order = new Order();
        Book book1 = new Book();
        Book book2 = new Book();
        Book book3 = new Book();
        book1.setAmount(2);
        book2.setAmount(1);
        book3.setAmount(0);
        order.getBooks().add(book1);
        order.getBooks().add(book2);
        order.getBooks().add(book3);

        Mockito.when(em.find(Book.class, book1.getId())).thenReturn(book1);
        Mockito.when(em.find(Book.class, book2.getId())).thenReturn(book2);
        Mockito.when(em.find(Book.class, book3.getId())).thenReturn(book3);

        OrdersService ordersService = new OrdersService(em);

        //Act
        try {
            ordersService.placeOrder(order);
        }
        //Assert
        catch (OutOfStockException e) {
            assertEquals(2, (int) book1.getAmount());
            assertEquals(1, (int) book2.getAmount());
        }
    }

    @Test(expected = OutOfStockException.class)
    public void whenOrderedMoreBooksThanAvailable_placeOrderThrowsOutOfStockEx() {
        //Arrange
        Order order = new Order();
        Book book = new Book();
        book.setAmount(1);
        order.getBooks().add(book);
        order.getBooks().add(book);

        Mockito.when(em.find(Book.class, book.getId())).thenReturn(book);

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert - exception expected
    }

    @Test
    public void whenOrderedBookAvailable_placeOrderDecreasesAmountByOne() {
        //Arrange
        Order order = new Order();
        Book book = new Book();
        book.setAmount(1);
        order.getBooks().add(book);

        Mockito.when(em.find(Book.class, book.getId())).thenReturn(book);

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert
        //dostępna liczba książek zmniejszyła się:
        assertEquals(0, (int)book.getAmount());
        //nastąpiło dokładnie jedno wywołanie em.persist(order) w celu zapisania zamówienia:
        Mockito.verify(em, times(1)).persist(order);
    }

    @Test
    public void whenOrderedBooksAvailable_placeOrderDecreasesAmount() {
        //Arrange
        Order order = new Order();
        Book book1 = new Book();
        Book book2 = new Book();
        book1.setAmount(3);
        book2.setAmount(3);
        order.getBooks().add(book1);
        order.getBooks().add(book1);
        order.getBooks().add(book2);

        Mockito.when(em.find(Book.class, book1.getId())).thenReturn(book1);
        Mockito.when(em.find(Book.class, book2.getId())).thenReturn(book2);

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert
        //dostępna liczba książek zmniejszyła się:
        assertEquals(1, (int)book1.getAmount());
        assertEquals(2, (int)book2.getAmount());
        //nastąpiło dokładnie jedno wywołanie em.persist(order) w celu zapisania zamówienia:
        Mockito.verify(em, times(1)).persist(order);
    }

    @Test(expected = EmptyOrderException.class)
    public void whenOrderEmpty_placeOrderThrowsEmptyOrderEx() {
        //Arrange
        Order order = new Order();

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert - exception expected
    }

    @Test(expected = TooManyProductsException.class)
    public void whenTooManyBooksInOrder_placeOrderThrowsTooManyProductsEx() {
        //Arrange
        Order order = new Order();
        List<Book> books = new ArrayList<>();
        books.add(new Book());
        books.add(new Book());
        books.add(new Book());
        books.add(new Book());
        books.add(new Book());

        books.get(0).setAmount(1);
        books.get(1).setAmount(1);
        books.get(2).setAmount(1);
        books.get(3).setAmount(1);
        books.get(4).setAmount(1);

        order.getBooks().addAll(books);

        Mockito.when(em.find(Book.class, books.get(0).getId())).thenReturn(books.get(0));
        Mockito.when(em.find(Book.class, books.get(1).getId())).thenReturn(books.get(1));
        Mockito.when(em.find(Book.class, books.get(2).getId())).thenReturn(books.get(2));
        Mockito.when(em.find(Book.class, books.get(3).getId())).thenReturn(books.get(3));
        Mockito.when(em.find(Book.class, books.get(4).getId())).thenReturn(books.get(4));

        OrdersService ordersService = new OrdersService(em);

        //Act
        ordersService.placeOrder(order);

        //Assert - exception expected
    }
}
